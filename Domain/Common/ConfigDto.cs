using System.ComponentModel.DataAnnotations;

namespace Domain.Common
{
    public record ConfigDto(
        [Required]
        string Key,
        [Required]
        string Value
        );
}
