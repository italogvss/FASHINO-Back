using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Configurations
    {
        [Key]
        public Guid Id { get; private set; }

        [Required]
        [MaxLength(100)]
        public string Key { get; private set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Value { get; private set; } = string.Empty;

        private Configurations() { }

        public Configurations(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public void UpdateValue(string newValue)
        {
            Value = newValue;
        }
    }
}
