using Domain.Enums.RevenueCat;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class SubscriptionEvent
    {
        [Key]
        public Guid Id { get; private set; }

        [Required]
        public Guid SubscriptionId { get; private set; }

        [ForeignKey(nameof(SubscriptionId))]
        public virtual Subscription Subscription { get; private set; } = null!;

        [Required]
        [MaxLength(200)]
        public string RevenueCatEventId { get; private set; } = string.Empty;

        [Required]
        public EventType EventType { get; private set; }

        [Required]
        public DateTime EventTimestamp { get; private set; }

        [Required]
        public string EventData { get; private set; } = string.Empty; // JSON completo do evento

        [Required]
        public DateTime ProcessedAt { get; private set; }

        public bool IsProcessed { get; private set; }

        private SubscriptionEvent() { }

        public SubscriptionEvent(Guid subscriptionId)
        {
            Id = Guid.NewGuid();
            SubscriptionId = subscriptionId;
            ProcessedAt = DateTime.UtcNow;
            IsProcessed = false;
        }

        public void MarkAsProcessed()
        {
            IsProcessed = true;
        }
    }
}
