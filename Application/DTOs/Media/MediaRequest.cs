using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.DTOs.Media
{
    public record MediaRequest
    {
        [JsonIgnore]
        public Guid UserId;
        [Required]
        public MediaCategory Category;
        public DateTime? StartDate = null;
        public DateTime? EndDate = null;
    };
}
