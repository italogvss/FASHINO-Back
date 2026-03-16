using Sahared.Common;

namespace Application.Interface.Services
{
    public interface IWebhookService
    {
        Task ProcessFashnAiWebhookAsync(FashnRunResponse payload);
    }
}
