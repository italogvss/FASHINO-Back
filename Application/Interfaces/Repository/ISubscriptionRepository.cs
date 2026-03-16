using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface ISubscriptionRepository : IRepository<Subscription>
    {
        Task<Subscription?> GetByUserId(Guid userId);
        Task<Subscription?> GetByOriginalTransactionIdAsync(string? originalTransactionId);
        Task<Subscription?> GetActiveByUserIdAsync(Guid userId);
    }
}
