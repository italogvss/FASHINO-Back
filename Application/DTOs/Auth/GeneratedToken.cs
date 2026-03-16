namespace Application.DTOs.Auth
{
    public class GeneratedToken
    {
        public DateTime AccessTokenExpiresAt;
        public DateTime RefreshTokenExpiresAt;
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
