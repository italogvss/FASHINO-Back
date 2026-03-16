using Application.Interfaces.Services;
using Sahared.Common;
using Worker.Configuration;
using Worker.Models.Worker.Models;
using Worker.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Worker.Services
{
    public class FashnAiService : IFashnAIService
    {
        private readonly IHttpClient httpClient;
        private readonly ILogger<FashnAiService> log;
        private readonly FashnAiConfiguration settings;

        public FashnAiService(IHttpClient httpClient, ILogger<FashnAiService> logger, IOptions<FashnAiConfiguration> settings)
        {
            this.httpClient = httpClient;
            log = logger;
            this.settings = settings.Value;
        }

        /// <summary>
        /// Vai enviar o job para a FASHN AI, retornando o ID externo do job criado ou runtime error, e null quando API-LEVEL error
        /// </summary>
        public async Task<FashnRunResponse?> SubmitJobAsync(FashnRunRequest request, CancellationToken cancellationToken = default)
        {
            if (settings.isDevelopment)
            {
                return await MockSubmitJobAsync();
            }

            try
            {
                //precisaria tratar isso aqui
                var json = JsonSerializer.Serialize(request);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync<StringContent, HttpResponseMessage>(
                    $"{settings.Url}/run?webhook_url={settings.WebhookUrl}",
                    content, cancellationToken);

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var error = JsonSerializer.Deserialize<FashnRunError>(responseContent)
                        ?? throw new InvalidOperationException("Failed to deserialize FASHN AI response");
                    return new FashnRunResponse { Error = error };
                }

                var result = JsonSerializer.Deserialize<FashnRunResponse>(responseContent)
                        ?? throw new InvalidOperationException("Failed to deserialize FASHN AI response");

                log.LogInformation("Job submitted to FASHN AI. External Job ID: {ExternalJobId}", result.Id);
                return result;
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error submitting job to FASHN AI");
                throw;
            }
        }

        private async Task<FashnRunResponse> MockSubmitJobAsync()
        {
            log.LogInformation("[MOCK] Simulando submissão de job à FASHN AI...");

            // Pequeno atraso para simular latência de rede
            await Task.Delay(Random.Shared.Next(200, 800));

            // 🔹 Simula chance de falha (ex: 10%)
            bool simulateError = Random.Shared.NextDouble() < 0.1;

            if (simulateError)
            {
                var errorResponse = new FashnRunResponse
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Status = "failed",
                    Error = new FashnRunError
                    {
                        Name = "MockProcessingError",
                        Message = "Erro simulado durante a geração da imagem (mock mode)."
                    }
                };

                log.LogWarning("[MOCK] Job simulado falhou: {Error}", errorResponse.Error.Message);
                return errorResponse;
            }

            var mockResponse = new FashnRunResponse
            {
                Id = Guid.NewGuid().ToString("N"),
                Error = null,

            };

            log.LogInformation("[MOCK] Job simulado criado com ID {ExternalJobId}", mockResponse.Id);
            return mockResponse;
        }

        public async Task<FashnRunResponse?> GetStatusAsync(string externalJobId, CancellationToken cancellationToken = default)
        {
            if (settings.isDevelopment)
            {
                return MockGetStatusAsync(externalJobId);
            }
            try
            {
                var url = $"{settings.Url}/status/{externalJobId}";
                log.LogInformation("Requesting job status from FASHN AI for Job ID: {ExternalJobId}", externalJobId);

                var response = await httpClient.GetAsync<HttpResponseMessage>(url, cancellationToken);

                if (!response.IsSuccessStatusCode)
                    return null;

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                var result = JsonSerializer.Deserialize<FashnRunResponse>(responseContent) ??
                    throw new InvalidOperationException("Failed to deserialize FASHN AI status response");

                log.LogInformation("FASHN AI job status received for Job {ExternalJobId}: {Status}", externalJobId, result.Status);

                return result;
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Unexpected error while getting status from FASHN AI for job {ExternalJobId}", externalJobId);
                throw;
            }
        }

        private FashnRunResponse MockGetStatusAsync(string id)
        {
            double random = Random.Shared.NextDouble();
            string status;
            List<string>? output = null;
            FashnRunError? error = null;

            // Distribui os cenários de forma lógica
            if (random < 0.6) // 60% - Sucesso
            {
                status = "completed";
                output = new List<string>
        {
            $"https://picsum.photos/id/{Random.Shared.Next(1, 100)}/1920/1080"
        };
            }
            else if (random < 0.85) // 25% - Processando
            {
                status = "processing";
            }
            else // 15% - Erro
            {
                status = "failed";
                error = new FashnRunError
                {
                    Name = "MockProcessingError",
                    Message = "Erro simulado durante a geração da imagem (mock mode)."
                };
            }

            return new FashnRunResponse
            {
                Id = id,
                Status = status,
                Output = output,
                Error = error
            };
        }


        public async Task<IFormFile> DownloadImageAsync(string firstImageUrl, CancellationToken cancellationToken)
        {
            try
            {
                var fileBytes = await httpClient.DownloadFileAsync(firstImageUrl, cancellationToken);

                if (fileBytes == null || fileBytes.Length == 0)
                    throw new InvalidOperationException("Imagem não encontrada ou vazia.");

                var fileName = Path.GetFileName(new Uri(firstImageUrl).LocalPath);
                var stream = new MemoryStream(fileBytes);

                var formFile = new FormFile(stream, 0, fileBytes.Length, "file", fileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/jpeg"
                };

                return formFile;
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Erro ao baixar imagem de {ImageUrl}", firstImageUrl);
                throw;
            }
        }
    }
}
