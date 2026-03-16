using Application.DTOs.Auth;
using System.Security.Claims;

namespace Application.Interfaces.Services
{
    public interface IAppTokenService
    {
        GeneratedToken GenerateToken(Guid userId, string email);
        ClaimsPrincipal? ValidateToken(string token);
        Guid? GetUserUidFromTokenAsync(string token);
    }
}
