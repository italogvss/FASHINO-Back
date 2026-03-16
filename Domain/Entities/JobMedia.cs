using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class JobMedia
    {
        [Key]
        public Guid Id { get; private set; }

        [Required]
        public Guid JobId { get; private set; }

        [ForeignKey(nameof(JobId))]
        public virtual Job Job { get; private set; } = null!;

        public Guid MediaId { get; private set; }

        [ForeignKey(nameof(MediaId))]
        public virtual Media Media { get; private set; }

        public Guid? JobExecutionId { get; private set; }

        [ForeignKey(nameof(JobExecutionId))]
        public virtual JobExecution? JobExecution { get; private set; }

        [Required]
        public string Url { get; private set; } = string.Empty;

        [Required]
        public DateTime UploadedAt { get; private set; } = DateTime.UtcNow;

        [Required]
        public MediaCategory Category { get; private set; }

        public JobMedia() { }

        public JobMedia(Guid jobId, Guid mediaId, Guid jobExecutionId, string url, MediaCategory category)
        {
            Id = Guid.NewGuid();
            JobId = jobId;
            MediaId = mediaId;
            UploadedAt = DateTime.UtcNow;
            Url = url;
            JobExecutionId = jobExecutionId;
            Category = category;
        }
    }
}
