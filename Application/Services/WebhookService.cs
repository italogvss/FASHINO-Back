using Application.Interface.Services;
using Application.Interfaces.Repository;
using Domain.Enums;
using Sahared.Common;
using Shared.Interface;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class WebhookService : IWebhookService
    {
        private readonly ILogger<WebhookService> _logger;
        private readonly IJobExecutionRepository executionRepository;
        public WebhookService(
            IJobExecutionRepository executionRepository,
            ILogger<WebhookService> logger)
        {
            _logger = logger;
            this.executionRepository = executionRepository;
        }

        public async Task ProcessFashnAiWebhookAsync(FashnRunResponse response)
        {

            _logger.LogInformation("Processing webhook for external job ID: {ExternalJobId}", response.Id);

            try
            {
                var execution = await executionRepository.GetByExternalIdAsync(response.Id);

                if (execution == null)
                {
                    _logger.LogWarning("JobExecution not found for external job ID: {ExternalJobId}", response.Id);
                    return;
                }

                if (execution.Status == JobStatus.Completed || execution.Status == JobStatus.Failed)
                {
                    _logger.LogInformation("Webhook ignored — JobExecution {ExecutionId} already finalized.", execution.Id);
                    return;
                }

                _logger.LogInformation("Processing webhook for JobExecution {ExecutionId}, status: {Status}",
                    execution.Id, response.Status);

                if (response.Status == "completed")
                {
                    BackgroundJob.Enqueue<ICompletedJob>(
                        job => job.ExecuteAsync(execution.Id, response, CancellationToken.None));
                }
                else if (response.Status == "failed")
                {
                    BackgroundJob.Enqueue<IFailedJob>(
                        job => job.ExecuteAsync(execution.Id, response.Error, CancellationToken.None));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing webhook for external job ID: {ExternalJobId}", response.Id);
                throw;
            }
        }
    }
}
