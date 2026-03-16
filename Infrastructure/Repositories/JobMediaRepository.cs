using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class JobMediaRepository : Repository<JobMedia>, IJobMediaRepository
    {
        public JobMediaRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<JobMedia>> GetByJobIdAsync(Guid jobId)
        {
            return await _dbSet
                .AsQueryable()
                .Include(jm => jm.Media)
                .Where(jm => jm.JobId == jobId)
                .ToListAsync();
        }
    }
}
