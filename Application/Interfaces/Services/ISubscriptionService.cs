using Application.DTOs.Subscription;

namespace Application.Interfaces.Services
{
    public interface ISubscriptionService
    {
        Task<SubscriptionResponse> GetUserSubscriptionsAsync(Guid userId);
        Task<SubscriptionResponse> SyncSubscriptionAsync(SubscriptionSyncRequest request);
        Task HandleRevenueCatWebhookAsync(RevenueCatWebhookRequest request);
    }
}
