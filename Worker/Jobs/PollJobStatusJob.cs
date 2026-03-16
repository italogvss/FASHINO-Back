using Domain.Common;
using Domain.Entities;
using Domain.Exceptions;
using Infrastructure.Persistence;
using Sahared.Common;
using Shared.Interface;
using Worker.Services.Interface;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Worker.Jobs
{
    public class PollJobStatusJob : IPollJob
    {
        private readonly AppDbContext context;
        private readonly IFashnAIService fashnAiService;
        private readonly ILogger<PollJobStatusJob> log;
        private AppConfigurations settings;
        private static readonly Random random = new();

        private JobExecution? execution = null;
        private Subscription? subscription = null;

        public PollJobStatusJob(
            AppDbContext context,
            IFashnAIService fashnAiService,
            ILogger<PollJobStatusJob> logger,
            IOptions<AppConfigurations> settings)
        {
            this.context = context;
            this.fashnAiService = fashnAiService;
            log = logger;
            this.settings = settings.Value;
        }
        [JobDisplayName("PollJobStatusJob")]
        public async Task ExecuteAsync(Guid jobExecutionId, int attemptNumber, CancellationToken cancellationToken = default)
        {
            log.LogInformation("Start polling for JobExecution {ExecutionId} (attempt {Attempt})", jobExecutionId, attemptNumber);

            try
            {
                execution = await GetExecutionAsync(jobExecutionId, cancellationToken);
                if (execution == null) return;
                if (execution.ExternalJobId == null) return;
                subscription = await GetSubscriptionAsync(jobExecutionId);
                if (subscription == null) return;

                FashnRunResponse response = await fashnAiService.GetStatusAsync(execution.ExternalJobId, cancellationToken);

                //API-LEVEL Error
                if (response == null)
                    throw new InvalidOperationException();
                if (response.Error != null)
                    throw new ExternalServiceException(response.Error.Name);

                log.LogInformation("External job {ExternalId} status = {Status}", execution.ExternalJobId, response.Status);

                //Runtime
                HandleStatusAsync(response, attemptNumber, cancellationToken);
            }
            catch (ExternalServiceException ex)
            {
                await HandleJobFailureAsync(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Unhandled error while polling JobExecution {ExecutionId}", jobExecutionId);
                ScheduleNextPollOrTimeoutAsync(attemptNumber, cancellationToken);
            }
        }

        private async Task<Subscription?> GetSubscriptionAsync(Guid jobExecutionId)
        {
            return await context.JobExecution
                            .Where(je => je.Id == jobExecutionId)
                            .Select(je => je.Job.User.Subscription)
                            .FirstOrDefaultAsync();
        }

        private async Task HandleJobFailureAsync(string? error)
        {

            if (execution == null)
                return;
            execution.Fail(error);
            if (subscription == null)
                return;
            subscription.ReleaseCredits(execution.UsedCredits);

            log.LogError(error, "Falha no Job ");
            await context.SaveChangesAsync();
        }

        private async Task<JobExecution?> GetExecutionAsync(Guid jobExecutionId, CancellationToken cancellationToken)
        {
            var execution = await context.JobExecution
                                    .Include(je => je.Job)
                                    .FirstOrDefaultAsync(je => je.Id == jobExecutionId, cancellationToken);

            if (execution == null)
                log.LogWarning("JobExecution {ExecutionId} not found", jobExecutionId);

            return execution;
        }

        private void HandleStatusAsync(FashnRunResponse response, int attemptNumber, CancellationToken cancellationToken)
        {
            var status = (response.Status ?? string.Empty).Trim().ToLowerInvariant();

            switch (status)
            {
                case "completed":
                    log.LogInformation("External job completed: {ExternalJobId}", response.Id);
                    BackgroundJob.Enqueue<ProcessCompletedJobJob>(job => job.ExecuteAsync(execution.Id, response, CancellationToken.None));
                    break;

                case "failed":
                    log.LogWarning("External job failed: {ExternalJobId} - scheduling failure processor", response.Id);
                    BackgroundJob.Enqueue<ProcessFailedJobJob>(job => job.ExecuteAsync(execution.Id, response.Error, CancellationToken.None));
                    break;

                case "processing":
                case "in_queue":
                case "starting":
                    ScheduleNextPollOrTimeoutAsync(attemptNumber, cancellationToken);
                    break;
                default:
                    break;
            }
        }

        private void ScheduleNextPollOrTimeoutAsync(int attemptNumber, CancellationToken cancellationToken)
        {
            var maxAttempts = settings.MaxJobRetry;

            if (attemptNumber >= maxAttempts)
            {
                log.LogWarning("Max polling attempts ({Max}) reached for JobExecution {ExecutionId}", maxAttempts, execution.Id);

                var timeoutError = new FashnRunError
                {
                    Name = "Timeout",
                    Message = "Job processing timeout - max polling attempts reached"
                };

                BackgroundJob.Enqueue<ProcessFailedJobJob>(job => job.ExecuteAsync(execution.Id, timeoutError, CancellationToken.None));
                return;
            }

            var baseInterval = settings.PollingIntervalSeconds;
            var backoffFactor = Math.Pow(2, attemptNumber - 1);
            var seconds = random.Next(0, Math.Min(5, (int)(baseInterval * backoffFactor)));
            var delay = TimeSpan.FromSeconds(seconds);

            log.LogInformation("Scheduling next poll for JobExecution {ExecutionId} in {Delay}s (attempt {NextAttempt})",
                execution.Id, delay, attemptNumber + 1);

            BackgroundJob.Schedule<PollJobStatusJob>(
                job => job.ExecuteAsync(execution.Id, attemptNumber + 1, CancellationToken.None),
                delay);
        }
    }
}
