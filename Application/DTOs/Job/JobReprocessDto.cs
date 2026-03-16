using Domain.Enums;

namespace Application.DTOs.Job
{
    public record JobReprocessDto(
    Guid JobId,
    Guid NewExecutionId,
    JobStatus Status,
    DateTime RequestedAt
);
}
