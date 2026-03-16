using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Tests
{
    public class CreateTestUser
    {
        public string? Uid { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        public string? DisplayName { get; set; }
        public string? Password { get; set; }
    }

}
