using Application.DTOs.Auth;
using Application.Interfaces.Services;
using Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthAppService service) : ControllerBase
    {

        [HttpPost("authenticate")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Autenticação",
            Description = "Autentica um usuário a partir do UID fornecido e retorna um token de acesso válido."
        )]
        public async Task<ApiResponse<AuthResponse>> Authenticate([FromBody] AuthRequest request)
        {
            var response = await service.AuthenticateAsync(request.IdToken);
            return ApiResponse<AuthResponse>.SuccessResult(response, "Login successful");
        }

        [HttpPost("admin")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Autenticação",
            Description = "Autentica um usuário a partir do UID fornecido e retorna um token de acesso válido."
        )]
        public async Task<ApiResponse<AuthResponse>> AuthAdmin([FromBody] AuthAdminRequest request)
        {
            var response = await service.AuthAdminAsync(request);
            return ApiResponse<AuthResponse>.SuccessResult(response, "Login successful");
        }

        [HttpPost("me")]
        [SwaggerOperation(
            Summary = "Dados do Usuario Logado",
            Description = "Retorna informações do usuario."
        )]

        public async Task<ApiResponse<UserInfo>> Me([FromBody] AuthRequest request)
        {
            var response = await service.GetAuthUserAsync(request.IdToken);
            return ApiResponse<UserInfo>.SuccessResult(response, "User info retrieved successfully");
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Refresh Token",
            Description = "Recebe um refresh token válido e retorna um novo par de tokens de autenticação."
        )]
        public async Task<ApiResponse<RefreshTokenResponse>> Refresh([FromBody] RefreshTokenRequest request)
        {
            var response = await service.RefreshTokenAsync(request);
            return ApiResponse<RefreshTokenResponse>.SuccessResult(response, "Token refreshed successfully");
        }

        [HttpPost("logout")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Fazer Logout",
            Description = "Invalida o refresh token fornecido, encerrando a sessão do usuário."
        )]
        public async Task<ApiResponse<object>> Logout([FromBody] RefreshTokenRequest request)
        {
            var response = await service.LogoutAsync(request);
            return ApiResponse<object>.SuccessResult(response, "Logout successful");
        }
    }
}