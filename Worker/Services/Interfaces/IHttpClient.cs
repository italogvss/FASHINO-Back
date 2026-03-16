namespace Application.Interfaces.Services
{
    public interface IHttpClient
    {
        Task<TResponse> PostAsync<TRequest, TResponse>(
            string endpoint,
            TRequest payload,
            CancellationToken cancellationToken = default);

        Task<TResponse> GetAsync<TResponse>(
            string endpoint,
            CancellationToken cancellationToken = default);

        Task<byte[]> DownloadFileAsync(
            string url,
            CancellationToken cancellationToken = default);
    }
}
