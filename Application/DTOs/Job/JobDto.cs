using Domain.Enums;

namespace Application.DTOs.Job
{
    public class JobDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public JobStatus Status { get; set; }
        public Guid UserId { get; set; }
        public List<JobResponse> Medias { get; set; } = new();
        public List<JobExecutionDto> Executions { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
