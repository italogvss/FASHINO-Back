namespace Application.DTOs.User
{
    public class UserDto
    {
        public Guid Id;
        public string FirebaseUid { get; set; } = string.Empty;
        public string? Email;
        public string? DisplayName;
        public DateTime CreatedAt;
        public DateTime LastLoginAt;
        public bool IsActive;
        public string? RefreshToken;
        public DateTime? RefreshTokenExpiresAt;
    }
}
