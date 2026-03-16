using System.Text.Json.Serialization;

namespace Application.DTOs.Webhooks
{
    public class RevenueCatWebhookDto
    {
        [JsonPropertyName("api_version")]
        public string ApiVersion { get; set; } = string.Empty;

        [JsonPropertyName("event")]
        public RevenueCatEventDto Event { get; set; } = new();
    }

    public class RevenueCatEventDto
    {
        // Campos obrigatórios
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("event_timestamp_ms")]
        public long EventTimestampMs { get; set; }

        [JsonPropertyName("app_id")]
        public string AppId { get; set; } = string.Empty;

        [JsonPropertyName("app_user_id")]
        public string AppUserId { get; set; } = string.Empty;

        [JsonPropertyName("original_app_user_id")]
        public string? OriginalAppUserId { get; set; }

        // Campos de assinatura
        [JsonPropertyName("product_id")]
        public string? ProductId { get; set; }

        [JsonPropertyName("entitlement_ids")]
        public List<string> EntitlementIds { get; set; } = new();

        [JsonPropertyName("period_type")]
        public string? PeriodType { get; set; }

        [JsonPropertyName("purchased_at_ms")]
        public long? PurchasedAtMs { get; set; }

        [JsonPropertyName("expiration_at_ms")]
        public long? ExpirationAtMs { get; set; }

        [JsonPropertyName("grace_period_expiration_at_ms")]
        public long? GracePeriodExpirationAtMs { get; set; }

        [JsonPropertyName("auto_resume_at_ms")]
        public long? AutoResumeAtMs { get; set; }

        [JsonPropertyName("store")]
        public string? Store { get; set; }

        [JsonPropertyName("environment")]
        public string Environment { get; set; } = string.Empty;

        // Campos de cancelamento/expiração
        [JsonPropertyName("cancel_reason")]
        public string? CancelReason { get; set; }

        [JsonPropertyName("expiration_reason")]
        public string? ExpirationReason { get; set; }

        // Campos financeiros
        [JsonPropertyName("price")]
        public decimal? Price { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("price_in_purchased_currency")]
        public decimal? PriceInPurchasedCurrency { get; set; }

        [JsonPropertyName("tax_percentage")]
        public decimal? TaxPercentage { get; set; }

        [JsonPropertyName("commission_percentage")]
        public decimal? CommissionPercentage { get; set; }

        // Outros campos importantes
        [JsonPropertyName("transaction_id")]
        public string? TransactionId { get; set; }

        [JsonPropertyName("original_transaction_id")]
        public string? OriginalTransactionId { get; set; }

        [JsonPropertyName("is_family_share")]
        public bool IsFamilyShare { get; set; }

        [JsonPropertyName("is_trial_conversion")]
        public bool? IsTrialConversion { get; set; }

        [JsonPropertyName("country_code")]
        public string? CountryCode { get; set; }

        [JsonPropertyName("offer_code")]
        public string? OfferCode { get; set; }

        [JsonPropertyName("presented_offering_id")]
        public string? PresentedOfferingId { get; set; }

        [JsonPropertyName("new_product_id")]
        public string? NewProductId { get; set; }

        [JsonPropertyName("aliases")]
        public List<string> Aliases { get; set; } = new();

        [JsonPropertyName("subscriber_attributes")]
        public Dictionary<string, object> SubscriberAttributes { get; set; } = new();
    }
}

