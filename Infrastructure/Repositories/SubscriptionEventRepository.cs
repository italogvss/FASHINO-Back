using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class SubscriptionEventRepository : Repository<SubscriptionEvent>, ISubscriptionEventRepository
    {
        public SubscriptionEventRepository(AppDbContext context) : base(context) { }

        public async Task<SubscriptionEvent?> GetByRevenueCatEventIdAsync(string eventId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(e => e.RevenueCatEventId == eventId);
        }

        public async Task<List<SubscriptionEvent>> GetBySubscriptionIdAsync(Guid subscriptionId)
        {
            return await _dbSet
                .Where(e => e.SubscriptionId == subscriptionId)
                .OrderBy(e => e.EventTimestamp)
                .ToListAsync();
        }
    }
}
