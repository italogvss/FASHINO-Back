using Application.DTOs.Tests;
using Domain.Common;

namespace Application.Interfaces.Services
{
    public interface IAuthService
    {

        Task<string> VerifyTokenAsync(string token);
        Task<AuthenticationUser> GetUserInfoAsync(string token);
        Task<string> CreateUserAsync(CreateTestUser request);
    }
}
