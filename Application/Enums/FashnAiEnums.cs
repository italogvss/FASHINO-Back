using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Application.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter<GarmentCategory>))]
    public enum GarmentCategory
    {
        [EnumMember(Value = "auto")]
        Auto,
        [EnumMember(Value = "bottoms")]
        Bottoms,
        [EnumMember(Value = "tops")]
        Tops,
        [EnumMember(Value = "one-pieces")]
        OnePieces
    }

    [JsonConverter(typeof(JsonStringEnumConverter<GarmentPhotoType>))]
    public enum GarmentPhotoType
    {
        [EnumMember(Value = "auto")]
        Auto,
        [EnumMember(Value = "flat-lay")]
        FlatLay,
        [EnumMember(Value = "model")]
        Model
    }

    [JsonConverter(typeof(JsonStringEnumConverter<OperationMode>))]
    public enum OperationMode
    {
        [EnumMember(Value = "performance")]
        Performance,
        [EnumMember(Value = "balanced")]
        Balanced,
        [EnumMember(Value = "quality")]
        Quality
    }
}
