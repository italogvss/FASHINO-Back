namespace Application.Interfaces.Services
{
    public interface IStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string? folder = null);
        Task<string> GetPreSignedUrlAsync(string objectKey, TimeSpan expiry);
        Task DeleteFileAsync(string objectKey);
        string GenerateObjectKey(string fileName, string? folder = null);
    }
}
