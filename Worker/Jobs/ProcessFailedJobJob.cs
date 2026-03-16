using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Sahared.Common;
using Shared.Interface;
using Shared.Interfaces;
using Worker.Configuration;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Worker.Jobs
{
    public class ProcessFailedJobJob : IFailedJob
    {
        private readonly AppDbContext context;
        private readonly IJobOrchestrationService orchestrationService;
        private readonly ILogger<ProcessFailedJobJob> logger;
        private readonly FashnAiConfiguration settings;

        public ProcessFailedJobJob(
            AppDbContext context,
            IJobOrchestrationService orchestrationService,
            ILogger<ProcessFailedJobJob> logger,
            IOptions<FashnAiConfiguration> fashnAiConfiguration)
        {
            this.context = context;
            this.orchestrationService = orchestrationService;
            this.logger = logger;
            settings = fashnAiConfiguration.Value;
        }
        [JobDisplayName("ProcessFailedJobJob")]
        public async Task ExecuteAsync(
            Guid jobExecutionId,
            FashnRunError? errorInfo,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Processing failed job for JobExecution {ExecutionId}", jobExecutionId);

            try
            {
                var execution = await GetJobExecutionAsync(jobExecutionId, cancellationToken);

                var subscription = await GetSubscriptionAsync(jobExecutionId, cancellationToken);

                var errorCode = errorInfo?.Name ?? "Unknown";
                var errorMessage = errorInfo?.Message ?? "Unknown error occurred";

                logger.LogWarning("JobExecution {ExecutionId} failed with error: {ErrorCode} - {ErrorMessage}",
                    jobExecutionId, errorCode, errorMessage);


                execution.Fail(errorMessage);

                await context.SaveChangesAsync(cancellationToken);

                logger.LogInformation("JobExecution {ExecutionId} marked as permanently failed", jobExecutionId);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing failed job for JobExecution {ExecutionId}", jobExecutionId);
                throw;
            }
        }

        private async Task<Subscription?> GetSubscriptionAsync(Guid jobExecutionId, CancellationToken cancellationToken)
        {
            var subscription = await context.JobExecution
                            .Where(je => je.Id == jobExecutionId)
                            .Select(je => je.Job.User.Subscription)
                            .FirstOrDefaultAsync(cancellationToken);

            if (subscription == null)
                throw new InvalidOperationException("Subscription não existe");

            return subscription;
        }

        private async Task<JobExecution?> GetJobExecutionAsync(Guid jobExecutionId, CancellationToken cancellationToken)
        {
            var execution = await context.JobExecution
                                    .Include(je => je.Job)
                                    .FirstOrDefaultAsync(je => je.Id == jobExecutionId, cancellationToken);

            if (execution == null)
                throw new InvalidOperationException("JobExecution não existe");
            if (execution.Status != JobStatus.Pending)
                throw new InvalidOperationException("Job em outra situação");

            logger.LogInformation("JobExecution validado {ExecutionId}", jobExecutionId);
            return execution;
        }
    }
}
