using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth
{
    public record AuthRequest(
        [Required(ErrorMessage = "IdToken is required")]
        string IdToken
    );
}
