using Application.DTOs.Auth;
using Application.DTOs.Tests;

namespace Application.Interfaces.Services
{
    public interface IAuthAppService
    {
        Task<AuthResponse> AuthAdminAsync(AuthAdminRequest request);
        Task<AuthResponse> AuthenticateAsync(string token);
        Task<string> CreateUserTestAsync(CreateTestUser request);
        Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request);
        Task<object> LogoutAsync(RefreshTokenRequest request);

        Task<UserInfo> GetAuthUserAsync(string id);
    }
}
