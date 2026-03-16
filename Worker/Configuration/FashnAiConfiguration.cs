namespace Worker.Configuration
{
    public class FashnAiConfiguration
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Url { get; set; } = "https://api.fashn.ai";
        public string WebhookUrl { get; set; } = string.Empty;
        public int MaxConcurrentRequests { get; set; } = 6;
        public int RunRateLimit { get; set; } = 50;
        public int StatusRateLimit { get; set; } = 50;
        public int DefaultTimeoutSeconds { get; set; } = 300;
        public int PollingIntervalSeconds { get; set; } = 30;
        public int MaxPollingAttempts { get; set; } = 20;
        public int WebhookTimeoutSeconds { get; set; } = 600;
        public bool isDevelopment { get; set; } = false;
    }
}
