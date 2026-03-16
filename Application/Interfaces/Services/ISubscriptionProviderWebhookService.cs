using Application.DTOs.Webhooks;

namespace Application.Interfaces.Services
{
    public interface ISubscriptionProviderWebhookService
    {
        Task ProcessWebhookAsync(RevenueCatWebhookDto payload);
    }
}
