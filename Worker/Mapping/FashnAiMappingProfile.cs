using Application.DTOs.Job;
using Worker.Models.Worker.Models;
using System.Reflection;
using System.Runtime.Serialization;

namespace Worker.Mapping
{
    public static class FashnRunMapper
    {
        public static FashnRunRequest ToFashnRunRequest(JobParameters parameters, string modelImageUrl, string garmentImageUrl)
        {
            return new FashnRunRequest
            {
                ModelName = parameters.Type,
                Inputs = new FashnInputs
                {
                    ModelImage = modelImageUrl,
                    GarmentImage = garmentImageUrl,
                    Category = parameters.Category.GetEnumMemberValue(),
                    GarmentPhotoType = parameters.GarmentPhotoType.GetEnumMemberValue(),
                    Mode = parameters.Mode.GetEnumMemberValue(),
                    Seed = parameters.Seed,
                    NumSamples = parameters.NumSamples,
                    OutputFormat = "jpeg"
                }
            };
        }

        // Helper para extrair o valor do atributo [EnumMember(Value = "...")]
        private static string GetEnumMemberValue<TEnum>(this TEnum enumValue)
            where TEnum : struct, Enum
        {
            var member = typeof(TEnum).GetMember(enumValue.ToString()).FirstOrDefault();
            var attr = member?.GetCustomAttribute<EnumMemberAttribute>();
            return attr?.Value ?? enumValue.ToString().ToLower();
        }
    }
}
