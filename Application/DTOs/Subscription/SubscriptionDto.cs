using Domain.Enums;
using System.Text.Json.Serialization;

namespace Application.DTOs.Subscription
{
    public class SubscriptionDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string ProductId { get; set; } = string.Empty;
        public string EntitlementId { get; set; } = string.Empty;

        public SubscriptionStatus Status { get; set; }

        public DateTime PurchasedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class SubscriptionResponse
    {
        public Guid Id { get; set; }
        public string ProductId { get; set; } = string.Empty;
        public string EntitlementId { get; set; } = string.Empty;
        public SubscriptionStatus Status { get; set; }
        public DateTime PurchasedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public int AvailableCredits { get; set; }
        public int AllocatedCredits { get; set; }
    }

    public class SubscriptionSyncRequest
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        public string ProductId { get; set; } = string.Empty;
        public string EntitlementId { get; set; } = string.Empty;
        public DateTime PurchasedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public int Credits { get; set; }
    }

    public class RevenueCatWebhookRequest
    {
        public string EventType { get; set; } = string.Empty;
        public string AppUserId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string EntitlementId { get; set; } = string.Empty;
        public long PurchasedAtMs { get; set; }
        public long ExpirationAtMs { get; set; }
    }

    public class ActivateTrialRequest
    {
        public Guid UserId { get; set; }
        public string ProductId { get; set; } = string.Empty;
        public string EntitlementId { get; set; } = string.Empty;
        public int TrialDays { get; set; }
        public int Credits { get; set; }
    }

}
