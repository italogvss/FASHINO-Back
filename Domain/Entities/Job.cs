using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Job
    {
        [Key]
        public Guid Id { get; private set; }

        [Required]
        public JobType Type { get; private set; }

        [Required]
        public Guid UserId { get; private set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; private set; } = null!;

        [Required]
        public JobStatus Status { get; private set; }

        // Relacionamento com mídias usadas
        public virtual ICollection<JobMedia> Medias { get; private set; } = new List<JobMedia>();

        // Relacionamento com histórico de execução
        public virtual ICollection<JobExecution> Executions { get; private set; } = new List<JobExecution>();

        [Required]
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public Job() { }

        public Job(JobType type, Guid userId)
        {
            Id = Guid.NewGuid();
            Type = type;
            UserId = userId;
            Status = JobStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        // Atualiza status geral do job
        public void UpdateStatus(JobStatus status)
        {
            Status = status;
        }

        // Associa mídias ao job
        public void AddMedia(JobMedia media)
        {
            Medias.Add(media);
        }

        // Associa uma execução (retry ou primeira tentativa)
        public void AddExecution(JobExecution execution)
        {
            Executions.Add(execution);
        }
    }

}
