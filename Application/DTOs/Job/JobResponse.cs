using Domain.Enums;

namespace Application.DTOs.Job
{
    public class JobResponse
    {
        public Guid MediaId { get; set; }
        public string Url { get; set; } = string.Empty;
        public MediaCategory Category { get; set; }
    }
}
