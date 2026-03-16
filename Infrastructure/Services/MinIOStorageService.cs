using Application.Interfaces.Services;
using Domain.Common;
using Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace Infrastructure.Services
{
    public class MinIOStorageService : IStorageService
    {
        private readonly IMinioClient minioClient;
        private readonly IMinioClient publicMinioClient;
        private readonly ILogger<MinIOStorageService> logger;
        private readonly string bucketName;
        private readonly MinIOSettings settings;

        public MinIOStorageService(IConfiguration configuration,
                                   ILogger<MinIOStorageService> logger,
                                   IOptions<MinIOSettings> settings)
        {
            this.logger = logger;
            this.settings = settings.Value;
            bucketName = this.settings.BucketName;
            minioClient = new MinioClient()
                .WithEndpoint(this.settings.Endpoint)
                .WithCredentials(this.settings.AccessKey, this.settings.SecretKey)
                .WithSSL(this.settings.UseSSL)
                .Build();

            publicMinioClient = new MinioClient()
                .WithEndpoint(this.settings.ExternalUrl)
                .WithCredentials(this.settings.AccessKey, this.settings.SecretKey)
                .WithSSL(this.settings.UseSSL)
                .Build();
            // Ensure bucket exists
            EnsureBucketExistsAsync().GetAwaiter().GetResult();
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string? folder = null)
        {
            try
            {
                var objectKey = GenerateObjectKey(fileName, folder);

                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectKey)
                    .WithStreamData(fileStream)
                    .WithObjectSize(fileStream.Length)
                    .WithContentType(contentType);

                await minioClient.PutObjectAsync(putObjectArgs);

                logger.LogInformation("File uploaded successfully to MinIO: {ObjectKey}", objectKey);
                return objectKey;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error uploading file {FileName} to MinIO", fileName);
                throw new ExternalServiceException("Erro ao fazer o upload do arquivo");
            }
        }

        public async Task<string> GetPreSignedUrlAsync(string objectKey, TimeSpan expiry)
        {
            try
            {
                var presignedGetObjectArgs = new PresignedGetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectKey)
                    .WithExpiry((int)expiry.TotalSeconds);

                var preSignedUrl = await publicMinioClient.PresignedGetObjectAsync(presignedGetObjectArgs);
                return preSignedUrl;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error generating pre-signed URL for {ObjectKey}", objectKey);
                throw;
            }
        }

        public async Task DeleteFileAsync(string objectKey)
        {
            try
            {
                var removeObjectArgs = new RemoveObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectKey);

                await minioClient.RemoveObjectAsync(removeObjectArgs);

                logger.LogInformation("File deleted successfully from MinIO: {ObjectKey}", objectKey);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting file {ObjectKey} from MinIO", objectKey);
                throw new ExternalServiceException("Erro ao deletar arquivo do sttorage");
            }
        }

        public string GenerateObjectKey(string fileName, string? folder = null)
        {
            var uniqueId = Guid.NewGuid().ToString();
            var timestamp = DateTimeOffset.UtcNow.ToString("yyyy/MM/dd");
            var extension = Path.GetExtension(fileName);
            var cleanFileName = Path.GetFileNameWithoutExtension(fileName);

            if (!string.IsNullOrEmpty(folder))
            {
                return $"{folder}/{timestamp}/{uniqueId}_{cleanFileName}{extension}";
            }

            return $"{timestamp}/{uniqueId}_{cleanFileName}{extension}";
        }

        private async Task EnsureBucketExistsAsync()
        {
            try
            {
                var bucketExistsArgs = new BucketExistsArgs().WithBucket(bucketName);
                var bucketExists = await minioClient.BucketExistsAsync(bucketExistsArgs);

                if (!bucketExists)
                {
                    var makeBucketArgs = new MakeBucketArgs().WithBucket(bucketName);
                    await minioClient.MakeBucketAsync(makeBucketArgs);
                    logger.LogInformation("Bucket {BucketName} created successfully", bucketName);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error ensuring bucket {BucketName} exists", bucketName);
                throw;
            }
        }
    }
}
