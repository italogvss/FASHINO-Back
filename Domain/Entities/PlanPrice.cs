using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class PlanPrice
    {
        [Key]
        public Guid Id { get; private set; }

        [Required]
        public Guid PlanId { get; private set; }

        [ForeignKey(nameof(PlanId))]
        public virtual Plan Plan { get; private set; }

        [Required]
        [MaxLength(100)]
        public string Region { get; private set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string Currency { get; private set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceMonthly { get; private set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceYearly { get; private set; }

        public PlanPrice() { }

        public PlanPrice(Plan plan, string region, string currency, decimal priceMonthly, decimal priceYearly)
        {
            Id = Guid.NewGuid();
            PlanId = plan.Id;
            Plan = plan;
            Region = string.IsNullOrWhiteSpace(region) ? throw new ArgumentNullException(nameof(region)) : region;
            Currency = string.IsNullOrWhiteSpace(currency) ? throw new ArgumentNullException(nameof(currency)) : currency;
            PriceMonthly = priceMonthly;
            PriceYearly = priceYearly;
        }

        public void UpdatePrices(decimal monthly, decimal yearly)
        {
            PriceMonthly = monthly;
            PriceYearly = yearly;
        }
    }
}
