using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Job
{
    public record JobRequest
    (
        [Required]
        Guid UserId,
        string? Type,
        string? Search,
        DateTime? From,
        DateTime? To,
        JobStatus? Status
    );
}
