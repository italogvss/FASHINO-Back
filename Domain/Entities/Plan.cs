using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Plan
    {
        [Key]
        public Guid Id { get; private set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; private set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; private set; }

        // Serialized features (JSON or comma-separated list)
        public string? Features { get; private set; }

        // Relação 1:N -> Plan tem muitos PlanPrice
        public virtual ICollection<PlanPrice> Prices { get; private set; } = new List<PlanPrice>();

        public Plan() { }

        public Plan(string name, string? description = null, string? features = null)
        {
            Id = Guid.NewGuid();
            Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentNullException(nameof(name)) : name;
            Description = description;
            Features = features;
        }

        public void UpdateDescription(string? description) => Description = description;
        public void UpdateFeatures(string? features) => Features = features;
    }
}
