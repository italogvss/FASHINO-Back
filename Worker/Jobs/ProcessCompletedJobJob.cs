using Application.DTOs.Media;
using Application.Interfaces.Persistence;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Sahared.Common;
using Shared.Interface;
using Worker.Services.Interface;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Worker.Jobs
{
    public class ProcessCompletedJobJob : ICompletedJob
    {
        private readonly AppDbContext context;
        private readonly IFashnAIService fashnAiService;
        private readonly IMediaService mediaService;
        private readonly ILogger<ProcessCompletedJobJob> logger;
        private readonly IUnitOfWork unit;

        public ProcessCompletedJobJob(
            AppDbContext context,
            IFashnAIService fashnAiService,
            IMediaService mediaService,
            ILogger<ProcessCompletedJobJob> logger,
            IUnitOfWork unit)
        {
            this.context = context;
            this.fashnAiService = fashnAiService;
            this.mediaService = mediaService;
            this.logger = logger;
            this.unit = unit;
        }

        [JobDisplayName("ProcessCompletedJob")]
        public async Task ExecuteAsync(Guid jobExecutionId, FashnRunResponse? responseData, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Processing completed job for JobExecution {ExecutionId}", jobExecutionId);

            try
            {
                var execution = await GetExecutionAsync(jobExecutionId, cancellationToken);
                if (execution == null) return;

                var imageUrls = responseData.Output ?? new List<string>();
                if (!imageUrls.Any())
                {
                    logger.LogWarning("No images found for JobExecution {ExecutionId}", jobExecutionId);
                    return;
                }

                await unit.BeginTransactionAsync();

                var uploadedMedias = await ProcessAndUploadImagesAsync(execution, imageUrls, cancellationToken);
                if (!uploadedMedias.Any())
                    throw new InvalidOperationException("No generated media to complete the job.");

                execution.Complete(uploadedMedias);

                await unit.CommitAsync();

                logger.LogInformation("JobExecution {ExecutionId} completed successfully", jobExecutionId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing completed job for JobExecution {ExecutionId}", jobExecutionId);
                throw;
            }
        }
        private async Task<JobExecution?> GetExecutionAsync(Guid jobExecutionId, CancellationToken cancellationToken)
        {
            var execution = await context.JobExecution
                .Include(je => je.Job)
                .FirstOrDefaultAsync(je => je.Id == jobExecutionId, cancellationToken);

            if (execution == null)
                logger.LogWarning("JobExecution {ExecutionId} not found", jobExecutionId);

            return execution;
        }

        // 🔹 Faz o download e upload das imagens geradas
        private async Task<List<JobMedia>> ProcessAndUploadImagesAsync(JobExecution execution, List<string> imageUrls, CancellationToken cancellationToken)
        {
            var jobMedias = new List<JobMedia>();

            foreach (var imageUrl in imageUrls)
            {
                var imageData = await DownloadImageSafelyAsync(imageUrl, execution.Id, cancellationToken);

                if (imageData == null)
                    continue;

                var fileName = $"job_{execution.JobId}_result_{DateTime.UtcNow:yyyyMMddHHmmss}.jpg";
                var uploadRequest = new MediaUploadRequest { UserId = execution.Job.UserId, File = imageData, Category = MediaCategory.Generated };

                MediaResponse? uploadedMedia = null;
                try
                {
                    uploadedMedia = await mediaService.UploadMediaAsync(uploadRequest);

                    var jobMedia = new JobMedia(
                        execution.JobId,
                        uploadedMedia.Id,
                        execution.Id,
                        uploadedMedia.Url,
                        MediaCategory.Generated
                    );

                    await context.JobMedia.AddAsync(jobMedia, cancellationToken);
                    jobMedias.Add(jobMedia);
                }
                catch
                {
                    logger.LogError("Failed to upload image for JobExecution {ExecutionId}", execution.Id);
                }
            }

            return jobMedias;
        }

        // 🔹 Download seguro com tratamento de erro
        private async Task<IFormFile?> DownloadImageSafelyAsync(string imageUrl, Guid jobExecutionId, CancellationToken cancellationToken)
        {
            try
            {
                return
                    await fashnAiService.DownloadImageAsync(imageUrl, cancellationToken);
            }
            catch (Exception ex) { return null; }
        }

    }
}