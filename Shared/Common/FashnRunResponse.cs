using System.Text.Json.Serialization;

namespace Sahared.Common
{
    public class FashnRunResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("output")]
        public List<string>? Output { get; set; }

        [JsonPropertyName("error")]
        public FashnRunError? Error { get; set; }
    }

    public class FashnRunError
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }
}
