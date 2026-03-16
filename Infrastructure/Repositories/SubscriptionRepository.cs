using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class SubscriptionRepository : Repository<Subscription>, ISubscriptionRepository
    {
        public SubscriptionRepository(AppDbContext context) : base(context) { }

        public Task<Subscription?> GetActiveByUserIdAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<Subscription?> GetByOriginalTransactionIdAsync(string? originalTransactionId)
        {
            throw new NotImplementedException();
        }

        public async Task<Subscription?> GetByUserId(Guid userId)
        {
            return await _dbSet.
                FirstOrDefaultAsync(s => s.UserId == userId);
        }
    }
}
