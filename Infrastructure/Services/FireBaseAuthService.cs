using AutoMapper;
using Application.DTOs.Tests;
using Application.Interfaces.Services;
using Domain.Common;
using Domain.Exceptions;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public class FirebaseAuthService : IAuthService
    {
        private readonly FirebaseAuth firebaseAuth;
        private readonly IMapper mapper;
        private readonly bool useEmulator;

        public FirebaseAuthService(FirebaseAuth firebaseAuth, IConfiguration configuration, IMapper mapper)
        {
            this.firebaseAuth = firebaseAuth ?? throw new ArgumentNullException(nameof(firebaseAuth));
            var useEmulator = configuration.GetSection("Firebase")["UseEmulator"];
            this.useEmulator = bool.Parse(useEmulator ?? "false");
            this.mapper = mapper;
        }

        public async Task<string> VerifyTokenAsync(string token)
        {
            if (useEmulator)
            {
                if (string.IsNullOrEmpty(token) || token.Length < 10)
                {
                    throw new AuthenticationException("Token inválido para emulador");
                }
                return token;
            }
            else
            {
                try
                {
                    var decodedToken = await firebaseAuth.VerifyIdTokenAsync(token);
                    return decodedToken.Uid;
                }
                catch (FirebaseAuthException ex)
                {
                    throw new AuthenticationException($"Token validation failed: {ex.Message}");
                }
                catch (Exception ex)
                {
                    throw new ExternalServiceException($"Firebase service error: {ex.Message}");
                }
            }
        }

        public async Task<AuthenticationUser> GetUserInfoAsync(string token)
        {
            if (useEmulator)
            {
                try
                {
                    // No emulador, o token é o UID
                    var userRecord = await firebaseAuth.GetUserAsync(token);
                    var user = mapper.Map<AuthenticationUser>(userRecord);
                    return user;
                }
                catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.UserNotFound)
                {
                    throw new NotFoundException($"User not found in emulator: {token}");
                }
                catch (FirebaseAuthException ex)
                {
                    throw new AuthenticationException($"Could not get user info from emulator: {ex.Message}");
                }
                catch (Exception ex)
                {
                    throw new ExternalServiceException($"Firebase emulator service error: {ex.Message}");
                }
            }
            else
            {
                try
                {
                    FirebaseToken decodedToken = await firebaseAuth.VerifyIdTokenAsync(token);
                    var userRecord = await firebaseAuth.GetUserAsync(decodedToken.Uid);
                    var user = mapper.Map<AuthenticationUser>(userRecord);
                    return user;
                }
                catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.InvalidIdToken)
                {
                    throw new AuthenticationException("Invalid Firebase token");
                }
                catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.UserNotFound)
                {
                    throw new NotFoundException($"User not found: {ex.Message}");
                }
                catch (FirebaseAuthException ex)
                {
                    throw new AuthenticationException($"Could not get user info: {ex.Message}");
                }
                catch (Exception ex)
                {
                    throw new ExternalServiceException($"Firebase service error: {ex.Message}");
                }
            }
        }

        public async Task<string> CreateUserAsync(CreateTestUser request)
        {
            try
            {
                var args = new UserRecordArgs
                {
                    Email = request.Email,
                    DisplayName = request.DisplayName,
                };

                if (!string.IsNullOrWhiteSpace(request.Password))
                    args.Password = request.Password;

                if (!string.IsNullOrWhiteSpace(request.Uid))
                    args.Uid = request.Uid;

                var userRecord = await firebaseAuth.CreateUserAsync(args);
                return userRecord.Uid;
            }
            catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.EmailAlreadyExists)
            {
                throw new ConflictException("Email already exists in Firebase");
            }
            catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.UidAlreadyExists)
            {
                throw new ConflictException("UID already exists in Firebase");
            }
            catch (FirebaseAuthException ex)
            {
                throw new ExternalServiceException($"Error creating Firebase user: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new ExternalServiceException($"Unexpected error creating Firebase user: {ex.Message}");
            }
        }
    }
}