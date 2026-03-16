using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth
{
    public record RefreshTokenRequest
    {
        [Required(ErrorMessage = "refresh token is required")]
        public string RefreshToken { get; set; } = string.Empty;
    };
}
