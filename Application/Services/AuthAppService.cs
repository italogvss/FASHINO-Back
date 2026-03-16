using Application.DTOs.Auth;
using Application.DTOs.Tests;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Services
{
    public class AuthAppService : IAuthAppService
    {
        private readonly IAuthService authService;
        private readonly IUserRepository userRepository;
        private readonly IAppTokenService tokenService;
        private readonly IMapper mapper;

        public AuthAppService(IAuthService authService,
                              IUserRepository userRepository,
                              IMapper mapper,
                              IAppTokenService tokenService)
        {
            this.authService = authService;
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.tokenService = tokenService;
        }

        public async Task<AuthResponse> AuthenticateAsync(string firebaseIdToken)
        {
            AuthenticationUser userInfo = await authService.GetUserInfoAsync(firebaseIdToken);
            User? user = await userRepository.GetByFirebaseUidAsync(userInfo.Uid);
            if (user == null)
            {
                user = new User(userInfo.Uid, userInfo.Email, userInfo.DisplayName);
                await userRepository.AddAsync(user);
            }
            else
            {
                user.UpdateProfile(userInfo.Email, userInfo.DisplayName);
                user.UpdateLoginTime();
            }

            GeneratedToken genToken = tokenService.GenerateToken(user.Id, user.Email ?? "");
            user.UpdateRefreshToken(genToken.RefreshToken, genToken.RefreshTokenExpiresAt);

            await userRepository.SaveChangesAsync();

            AuthResponse response = new(
                genToken.Token,
                genToken.RefreshTokenExpiresAt,
                genToken.RefreshToken,
                genToken.RefreshTokenExpiresAt
            );

            return response;
        }

        public async Task<AuthResponse> AuthAdminAsync(AuthAdminRequest request)
        {
            if(request.username != "italogvss" && request.password != "#Olati2000")
            {
                throw new AuthenticationException("Invalid username or password");
            }

            GeneratedToken genToken = tokenService.GenerateToken(new Guid(), request.username);

            AuthResponse response = new(
                genToken.Token,
                genToken.RefreshTokenExpiresAt,
                genToken.RefreshToken,
                genToken.RefreshTokenExpiresAt
            );

            return response;
        }

        public async Task<UserInfo> GetAuthUserAsync(string firebaseUid)
        {
            User? user = await userRepository.GetByFirebaseUidAsync(firebaseUid);
            if (user == null)
                throw new NotFoundException("Usuário não encontrado.");
            return mapper.Map<UserInfo>(user);
        }

        public async Task<string> CreateUserTestAsync(CreateTestUser request)
        {

            var uid = await authService.CreateUserAsync(request);

            if (string.IsNullOrEmpty(uid))
                throw new ExternalServiceException("Firebase returned empty UID after user creation.");

            var existingUser = await userRepository.GetByFirebaseUidAsync(uid);
            if (existingUser == null)
            {
                var user = new User(uid, request.Email, request.DisplayName);
                await userRepository.AddAsync(user);
                return uid;
            }

            return string.Empty;
        }
        public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var user = await userRepository.GetByRefreshTokenAsync(request.RefreshToken);

            if (user == null || !user.IsRefreshTokenValid(request.RefreshToken))
            {
                throw new AuthenticationException("Invalid or expired refresh token");
            }

            GeneratedToken newAccessToken = tokenService.GenerateToken(user.Id, user.Email ?? "");

            user.UpdateRefreshToken(newAccessToken.RefreshToken, newAccessToken.RefreshTokenExpiresAt);
            await userRepository.UpdateAsync(user);

            var response = new RefreshTokenResponse(newAccessToken.Token, newAccessToken.RefreshTokenExpiresAt);

            return response;
        }

        public async Task<object> LogoutAsync(RefreshTokenRequest request)
        {
            var user = await userRepository.GetByRefreshTokenAsync(request.RefreshToken);

            if (user == null)
            {
                return "Logout successful";
            }

            user.RevokeRefreshToken();
            await userRepository.UpdateAsync(user);

            return "Logout successful";
        }
    }
}