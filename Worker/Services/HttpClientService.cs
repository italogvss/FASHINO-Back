using Application.Interfaces.Services;
using System.Text;
using System.Text.Json;

namespace Worker.Services
{
    public class HttpClientService : IHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly SemaphoreSlim _rateLimiter;

        public HttpClientService(HttpClient httpClient, ILogger<HttpClientService> logger)
        {
            _httpClient = httpClient;
            _rateLimiter = new SemaphoreSlim(6, 6);
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(
            string endpoint,
            TRequest payload,
            CancellationToken cancellationToken = default)
        {
            await _rateLimiter.WaitAsync(cancellationToken);
            try
            {
                var json = JsonSerializer.Serialize(payload);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
                response.EnsureSuccessStatusCode();

                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonSerializer.Deserialize<TResponse>(body)!;
            }
            finally
            {
                _rateLimiter.Release();
            }
        }

        public async Task<TResponse> GetAsync<TResponse>(
            string endpoint,
            CancellationToken cancellationToken = default)
        {
            await _rateLimiter.WaitAsync(cancellationToken);
            try
            {
                using var response = await _httpClient.GetAsync(endpoint, cancellationToken);
                response.EnsureSuccessStatusCode();

                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonSerializer.Deserialize<TResponse>(body)!;
            }
            finally
            {
                _rateLimiter.Release();
            }
        }

        public async Task<byte[]> DownloadFileAsync(string url, CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync(cancellationToken);
        }
    }
}
