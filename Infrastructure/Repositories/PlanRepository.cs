using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PlanRepository : Repository<Plan>, IPlanRepository
    {
        public PlanRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Plan>> GetAllByRegionAsync(string region)
        {
            return await _dbSet
                .Include(p => p.Prices.Where(pp => pp.Region == region))
                .Where(p => p.Prices.Any(pp => pp.Region == region))
                .ToListAsync();
        }
        public async Task<Plan?> GetByIdWithRegionAsync(Guid id, string region)
        {
            return await _dbSet
                .Include(p => p.Prices.Where(pp => pp.Region == region))
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
