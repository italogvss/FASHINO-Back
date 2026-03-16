using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class JobExecution
    {
        [Key]
        public Guid Id { get; private set; }

        [Required]
        public Guid JobId { get; private set; }

        [ForeignKey(nameof(JobId))]
        public virtual Job Job { get; private set; } = null!;

        [MaxLength(100)]
        public string? ExternalJobId { get; private set; } // ID da API externa (Fashn.AI)

        [Required]
        public JobStatus Status { get; private set; } = JobStatus.Pending;

        [Column(TypeName = "text")]
        public string? InputParameters { get; private set; }

        [MaxLength(500)]
        public virtual List<JobMedia> ResultImages { get; private set; }

        [MaxLength(500)]
        public string? ErrorMessage { get; set; }

        public int UsedCredits { get; set; }
        public int RetryCount { get; set; }
        public int MaxRetries { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? StartedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }

        public JobExecution() { }

        public JobExecution(Guid jobId, string? inputParameters = null, int maxRetries = 3, int usedCredits = 0)
        {
            Id = Guid.NewGuid();
            JobId = jobId;
            InputParameters = inputParameters;
            MaxRetries = maxRetries;
            Status = JobStatus.Pending;
            CreatedAt = DateTime.UtcNow;
            RetryCount = 0;
            ResultImages = new List<JobMedia>();
            UsedCredits = usedCredits;
        }

        public void Start(string externalJobId)
        {
            Status = JobStatus.Processing;
            StartedAt = DateTime.UtcNow;
            ExternalJobId = externalJobId;
            Job.UpdateStatus(JobStatus.Processing);
        }

        public void Complete(List<JobMedia> Images)
        {
            if (!Images.Any())
                throw new ArgumentException("Images list cannot be empty when completing a job execution.");
            Status = JobStatus.Completed;
            ResultImages = Images;
            CompletedAt = DateTime.UtcNow;
            Job.UpdateStatus(JobStatus.Completed);
        }

        public void Cancel(string? reason = null)
        {
            Status = JobStatus.Cancelled;
            ErrorMessage = reason;
            CompletedAt = DateTime.UtcNow;
            Job.UpdateStatus(JobStatus.Cancelled);
        }

        public void Fail(string? errorMessage = null)
        {
            Status = JobStatus.Failed;
            ErrorMessage = errorMessage;
            RetryCount++;
            CompletedAt = DateTime.UtcNow;
            Job.UpdateStatus(JobStatus.Failed);
        }
    }
}
