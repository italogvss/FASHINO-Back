using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter<JobType>))]
    public enum JobType
    {
        [EnumMember(Value = "tryon-v1.6")]
        TryOn,
        [EnumMember(Value = "model-create")]
        ModelCreating,
        [EnumMember(Value = "product-to-model")]
        ProductToModel,
        [EnumMember(Value = "face-to-model")]
        FaceToModel,
    }
}
