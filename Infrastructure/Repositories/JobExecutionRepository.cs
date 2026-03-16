using Application.Interfaces.Repository;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class JobExecutionRepository : Repository<JobExecution>, IJobExecutionRepository
    {
        public JobExecutionRepository(AppDbContext context) : base(context) { }

        public async Task<JobExecution?> GetLatestExecutionForJob(Guid jobId)
        {
            return await _dbSet
                .AsQueryable()
                .Where(e => e.JobId == jobId)
                .OrderByDescending(e => e.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<JobExecution>> GetPendingExecutionsAsync(int limit)
        {
            return await _dbSet
                .AsQueryable()
                .Include(e => e.Job)
                .Where(e => e.Status == JobStatus.Pending)
                .OrderBy(e => e.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<bool> HasPendingJobsAsync(Guid userId)
        {
            return await _dbSet
                            .AsQueryable()
                            .AnyAsync(e => e.Job.UserId == userId && e.Status == JobStatus.Pending);
        }

        public async Task<JobExecution?> GetByExternalIdAsync(string id)
        {
            return await _dbSet.AsQueryable().Where(e => e.ExternalJobId == id).FirstOrDefaultAsync();
        }
    }
}
