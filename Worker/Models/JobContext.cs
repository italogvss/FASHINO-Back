namespace Worker.Models
{
    public class JobContext
    {
        public Guid JobId { get; set; }
        public Guid JobExecutionId { get; set; }
        public string ModelName { get; set; } = string.Empty;
        public Dictionary<string, string> MediaPaths { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
    }
}
