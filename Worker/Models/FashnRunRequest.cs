using Domain.Enums;
using System.Text.Json.Serialization;

namespace Worker.Models
{
    namespace Worker.Models
    {
        public class FashnRunRequest
        {
            [JsonPropertyName("model_name")]
            public JobType ModelName { get; set; }

            [JsonPropertyName("inputs")]
            public FashnInputs Inputs { get; set; } = new();
        }

        public class FashnInputs
        {
            [JsonPropertyName("model_image")]
            public string ModelImage { get; set; } = string.Empty;

            [JsonPropertyName("garment_image")]
            public string GarmentImage { get; set; } = string.Empty;

            [JsonPropertyName("product_image")]
            public string ProductImage { get; set; } = string.Empty;

            [JsonPropertyName("category")]
            public string Category { get; set; } = "auto";

            [JsonPropertyName("garment_photo_type")]
            public string GarmentPhotoType { get; set; } = "auto";

            [JsonPropertyName("mode")]
            public string Mode { get; set; } = "balanced";

            [JsonPropertyName("seed")]
            public int Seed { get; set; } = 1;

            [JsonPropertyName("num_samples")]
            public int NumSamples { get; set; } = 1;

            [JsonPropertyName("output_format")]
            public string OutputFormat { get; set; } = "jpeg";
        }
    }
}
