using Application.DTOs.Auth;
using Application.Interfaces.Services;
using Domain.Common;
using Domain.Exceptions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace Infrastructure.Services
{
    public class AppTokenService : IAppTokenService
    {
        private readonly TokenSettings tokenSettings;

        public AppTokenService(IOptions<TokenSettings> tokenSettings)
        {
            this.tokenSettings = tokenSettings.Value;
        }

        public GeneratedToken GenerateToken(Guid userId, string email)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: tokenSettings.Issuer,
                audience: tokenSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(tokenSettings.ExpiryInMinutes),
                signingCredentials: credentials
            );

            var refreshToken = GenerateRefreshToken();

            return new GeneratedToken
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(tokenSettings.ExpiryInMinutes),
                RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(tokenSettings.RefreshTokenExpiryInDays),
                RefreshToken = refreshToken
            };
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public Guid? GetUserUidFromTokenAsync(string token)
        {
            var principal = ValidateToken(token);
            if (principal == null) return null;

            var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId)) return null;
            return userId;
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(tokenSettings.SecretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = tokenSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = tokenSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // Remove tolerância de tempo
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch (SecurityTokenExpiredException)
            {
                throw new AuthenticationException("Token has expired");
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                throw new AuthenticationException("Invalid token signature");
            }
            catch (SecurityTokenValidationException ex)
            {
                throw new AuthenticationException($"Token validation failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new AuthenticationException($"Token validation error: {ex.Message}");
            }
        }
    }

}
