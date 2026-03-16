using Domain.Enums;

namespace Application.DTOs.Job
{
    public class JobExecutionDto
    {
        public Guid Id { get; set; }
        public string? ExternalJobId { get; set; }
        public JobStatus Status { get; set; }
        public string? ErrorMessage { get; set; }
        public int UsedCredits { get; set; }
        public int RetryCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
