using System.Text.Json.Serialization;

namespace Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SubscriptionStatus
    {
        Active = 1,     // Assinatura ativa e vigente
        Expired = 2,    // Assinatura expirou naturalmente
        Cancelled = 3,  // Assinatura cancelada pelo usuário ou admin
        PastDue = 4,    // Assinatura em atraso (ex: pagamento falhou, mas ainda dentro do período de carência)
        Paused = 5      // Assinatura pausada temporariamente
    }
}
