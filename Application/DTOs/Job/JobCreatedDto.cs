using Domain.Enums;

namespace Application.DTOs.Job
{
    public record JobCreatedDto(
        Guid Id,
        JobStatus Status,
        DateTime CreatedAt
    );
}