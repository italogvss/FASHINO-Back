using Domain.Enums;

namespace Application.DTOs.Media
{
    public class MediaResponse
    {
        public Guid Id { get; set; }
        public MediaCategory Category { get; set; }
        public string Url { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
