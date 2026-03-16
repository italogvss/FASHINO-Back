using System.Text.Json.Serialization;

namespace Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MediaCategory
    {
        Garment,
        Model,
        Generated,
        Other,
        Face,
        Product
    }
}
