using Domain.Common;
using Domain.Enums;
using Infrastructure.Persistence;
using Shared.Interface;
using Worker.Models;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Worker.Jobs
{
    public class CleanupExpiredJobsJob : ICleanUpJob
    {
        private readonly AppDbContext _context;
        private readonly IOptionsMonitor<AppConfigurations> _configMonitor;
        private readonly ILogger<CleanupExpiredJobsJob> _logger;

        public CleanupExpiredJobsJob(
            AppDbContext context,
            IOptionsMonitor<AppConfigurations> configMonitor,
            ILogger<CleanupExpiredJobsJob> logger,
            IOptions<WorkerConfigs> workerCOnfig)
        {
            _context = context;
            _configMonitor = configMonitor;
            _logger = logger;
        }
        [JobDisplayName("CleanupExpiredJobsJob")]
        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting cleanup of expired jobs");

            try
            {
                var expirationHours = 3;
                var expirationDate = DateTime.UtcNow.AddHours(-expirationHours);

                // Busca jobs antigos completados ou falhados
                var expiredJobs = await _context.Jobs
                    .Where(j => (j.Status == JobStatus.Completed || j.Status == JobStatus.Failed)
                             && j.CreatedAt < expirationDate)
                    .Include(j => j.Executions)
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("Found {Count} expired jobs to cleanup", expiredJobs.Count);

                foreach (var job in expiredJobs)
                {
                    try
                    {
                        // Aqui você pode adicionar lógica para deletar mídias temporárias
                        // Por exemplo, deletar do MinIO imagens geradas antigas

                        _logger.LogInformation("Cleaned up job {JobId}", job.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error cleaning up job {JobId}", job.Id);
                    }
                }

                _logger.LogInformation("Cleanup completed. Processed {Count} jobs", expiredJobs.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during cleanup job");
                throw;
            }
        }
    }
}
