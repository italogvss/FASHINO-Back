using Application.Enums;
using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.DTOs.Job
{
    public record CreateJobRequest
    {
        [Required]
        public JobType Type;

        [JsonIgnore]
        public Guid UserId;

        [Required]
        public JobParameters Parameters;
    };

    public record JobParameters
    (
        [Required]
        JobType Type,

        [Required]
        IEnumerable<Guid> MediaIds,

        [Required]
        GarmentPhotoType GarmentPhotoType,

        [Required]
        GarmentCategory Category,

        [Required]
        OperationMode Mode,

        [Range(0,int.MaxValue)]
        int Seed,

        [Required]
        [Range(1,4)]
        int NumSamples,

        string? Prompt,

        [Required]
        [Range(0,9999)]
        int PhotoPrice,

        [Required]
        string AspectRatio
    );
}
