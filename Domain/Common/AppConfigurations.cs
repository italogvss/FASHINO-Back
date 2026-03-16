namespace Domain.Common
{
    public class AppConfigurations
    {
        public int MaxFileSizeMB { get; set; } = 0;
        public string AllowedMimes { get; set; } = string.Empty;
        public int MaxJobRetry { get; set; } = 0;
        public int PollingIntervalSeconds { get; set; } = 15;
    }
}
