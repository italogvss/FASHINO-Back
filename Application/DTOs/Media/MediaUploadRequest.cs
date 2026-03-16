using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.DTOs.Media
{
    public class MediaUploadRequest
    {
        [JsonIgnore]
        public Guid UserId;
        [Required]
        public IFormFile File;
        [Required]
        public MediaCategory Category;
    };
}
