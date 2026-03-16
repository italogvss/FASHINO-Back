using Infrastructure.Persistence;
using Shared.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Worker.Services
{
    public class JobOrchestrationService : IJobOrchestrationService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<JobOrchestrationService> _logger;

        public JobOrchestrationService(
            AppDbContext context,
            ILogger<JobOrchestrationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> ShouldRetryAsync(Guid jobExecutionId, string errorName, CancellationToken cancellationToken = default)
        {
            var execution = await _context.JobExecution
                .Where(je => je.Id == jobExecutionId)
                .Select(je => new { je.Id, je.RetryCount, je.MaxRetries })
                .FirstOrDefaultAsync(cancellationToken);

            if (execution == null)
                return false;

            var noRetryErrors = new[] { "BadRequest", "UnauthorizedAccess", "OutOfCredits", "InvalidInput", "NotFound", "InternalServerError" };
            if (noRetryErrors.Contains(errorName))
            {
                _logger.LogInformation("Error code {ErrorCode} is not retryable", errorName);
                return false;
            }

            if (execution.RetryCount >= execution.MaxRetries)
            {
                _logger.LogInformation("Max retry attempts ({MaxRetries}) reached for execution {ExecutionId}",
                    execution.MaxRetries, jobExecutionId);
                return false;
            }

            return true;
        }
    }
}
