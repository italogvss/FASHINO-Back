namespace Application.DTOs.Auth
{
    public record AuthResponse(
        string Token,
        DateTime ExpiresAt,
        string RefreshToken,
        DateTime RefreshTokenExpiresAt
    );
}
