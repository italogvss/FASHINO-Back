using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Subscription
    {
        [Key]
        public Guid Id { get; private set; }

        [Required]
        public Guid UserId { get; private set; }
        public virtual User User { get; private set; } = null!;

        [Required]
        [MaxLength(100)]
        public string ProductId { get; private set; } = string.Empty; // ex: "premium_monthly"

        [Required]
        [MaxLength(100)]
        public string EntitlementId { get; private set; } = string.Empty; // ex: "pro"

        [Required]
        public SubscriptionStatus Status { get; private set; } = SubscriptionStatus.Active;

        [Required]
        public int AvailableCredits { get; private set; }

        [Required]
        public int AllocatedCredits { get; private set; }

        [Required]
        public DateTime PurchasedAt { get; private set; }

        public DateTime? ExpiresAt { get; private set; }

        [Required]
        public DateTime CreatedAt { get; private set; }

        [Required]
        public DateTime UpdatedAt { get; private set; }
        public Subscription() { }

        public Subscription(Guid userId, string productId, string entitlementId, int availableCredits, DateTime purchasedAt, DateTime? expiresAt)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            ProductId = productId;
            EntitlementId = entitlementId;
            PurchasedAt = purchasedAt;
            ExpiresAt = expiresAt;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            AvailableCredits = availableCredits;
            AllocatedCredits = 0;

        }
        public void Update(string productId, string entitlementId, DateTime? expiresAt, SubscriptionStatus status)
        {
            ProductId = productId;
            EntitlementId = entitlementId;
            ExpiresAt = expiresAt;
            Status = status;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool HasAvailableCredits(int requiredCredits)
        {
            return Status == SubscriptionStatus.Active &&
                   (ExpiresAt is null || ExpiresAt > DateTime.UtcNow) &&
                   (AllocatedCredits + requiredCredits) <= AvailableCredits;
        }


        public void AllocateCredits(int value)
        {
            if (value < 0)
                throw new ArgumentException("Value must be positive.", nameof(value));
            if ((value + AllocatedCredits) > AvailableCredits)
                throw new InvalidOperationException("Insufficient available credits to allocate.");
            AllocatedCredits += value;
        }

        public void addCredits(int value)
        {
            if (value < 0)
                throw new ArgumentException("Value must be positive.", nameof(value));
            AvailableCredits += value;
        }
        public void ConsumeAllocatedCredits()
        {
            AvailableCredits -= AllocatedCredits;
        }

        public void ReleaseCredits(int value)
        {
            if (value <= 0)
                throw new ArgumentException("O valor precisa ser positivo.", nameof(value));

            if (value > AllocatedCredits)
                throw new InvalidOperationException("Tentativa de liberar mais créditos do que o alocado.");

            AllocatedCredits -= value;
        }
    }
}
