using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface IJobExecutionRepository : IRepository<JobExecution>
    {
        Task<IEnumerable<JobExecution>> GetPendingExecutionsAsync(int limit);
        Task<JobExecution?> GetLatestExecutionForJob(Guid jobId);
        Task<bool> HasPendingJobsAsync(Guid userId);

        Task<JobExecution?> GetByExternalIdAsync(string id);
    }
}
