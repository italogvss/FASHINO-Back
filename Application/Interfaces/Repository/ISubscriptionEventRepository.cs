using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface ISubscriptionEventRepository : IRepository<SubscriptionEvent>
    {
        Task<SubscriptionEvent?> GetByRevenueCatEventIdAsync(string eventId);
        Task<List<SubscriptionEvent>> GetBySubscriptionIdAsync(Guid subscriptionId);
    }
}
