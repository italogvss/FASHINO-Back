using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Media
    {
        public Guid Id { get; private set; }

        [Required]
        [MaxLength(512)]
        public string ObjectKey { get; private set; } = string.Empty;

        [Required]
        [MaxLength(128)]
        public string MimeType { get; private set; } = string.Empty;

        public long Size { get; private set; }

        [MaxLength(256)]
        public string? FileName { get; private set; }

        [Required]
        public MediaCategory Category { get; private set; }


        public Guid? UserId { get; private set; }

        [ForeignKey(nameof(UserId))]
        public virtual User? User { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public Media() { }

        public Media(string objectKey, string mimeType, long size, MediaCategory category, string? fileName = null, Guid? userId = null)
        {
            Id = Guid.NewGuid();
            ObjectKey = objectKey;
            MimeType = mimeType;
            Size = size;
            Category = category;
            FileName = fileName;
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
