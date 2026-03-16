using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using Application.Services;
using AutoMapper;
using Moq;

namespace Tests.Application.Services
{
    [TestFixture]
    public class AuthAppServiceTests
    {
        private Mock<IAuthService> _authServiceMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IAppTokenService> _tokenServiceMock;
        private Mock<IMapper> _mapperMock;
        private AuthAppService _service;

        [SetUp]
        public void SetUp()
        {
            // Inicialize os mocks
            _authServiceMock = new Mock<IAuthService>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _tokenServiceMock = new Mock<IAppTokenService>();
            _mapperMock = new Mock<IMapper>();

            // Crie a instância real do serviço com os mocks
            _service = new AuthAppService(
                _authServiceMock.Object,
                _userRepositoryMock.Object,
                _mapperMock.Object,
                _tokenServiceMock.Object
            );
        }

        [Test]
        public async Task AuthenticateAsync_Should_Update_Existing_User()
        {
            // Arrange
            // - Mockar userRepository.GetByFirebaseUidAsync() para retornar um usuário existente.
            // - Mockar authService.GetUserInfoAsync() para retornar novos dados.
            // - Mockar tokenService.GenerateToken() e GenerateRefreshToken() para retornos previsíveis.
            // - Mockar mapper.Map().

            // Act
            // - Executar AuthenticateAsync(firebaseUid).

            // Assert
            // - Verificar se user.UpdateProfile() e user.UpdateLoginTime() foram chamados.
            // - Verificar se userRepository.UpdateAsync() e SaveChangesAsync() foram executados.
        }

        [Test]
        public async Task CreateUserTestAsync_Should_Create_User_When_Not_Exists()
        {
            // Arrange
            // - Mockar authService.CreateUserAsync() para retornar um UID.
            // - Mockar userRepository.GetByFirebaseUidAsync() para retornar null.

            // Act
            // - Chamar CreateUserTestAsync() com um CreateTestUser válido.

            // Assert
            // - Verificar se userRepository.AddAsync() foi chamado com User correto.
            // - Verificar se o método retornou o UID gerado.
        }

        [Test]
        public void CreateUserTestAsync_Should_Throw_When_Firebase_Returns_EmptyUid()
        {
            // Arrange
            // - Mockar authService.CreateUserAsync() para retornar string vazia.

            // Act + Assert
            // - Verificar se o método lança ExternalServiceException.
        }

        [Test]
        public async Task RefreshTokenAsync_Should_Return_New_Tokens_When_Valid()
        {
            // Arrange
            // - Mockar userRepository.GetByRefreshTokenAsync() para retornar um usuário válido.
            // - Mockar tokenService para retornar novos tokens e datas de expiração.

            // Act
            // - Executar RefreshTokenAsync().

            // Assert
            // - Verificar se user.UpdateRefreshToken() foi chamado.
            // - Verificar se userRepository.UpdateAsync() foi chamado.
            // - Verificar se o retorno contém o novo token e data de expiração.
        }

        [Test]
        public void RefreshTokenAsync_Should_Throw_When_Token_Invalid_Or_Expired()
        {
            // Arrange
            // - Mockar userRepository.GetByRefreshTokenAsync() para retornar null
            //   ou um usuário cujo IsRefreshTokenValid() retorna false.

            // Act + Assert
            // - Garantir que AuthenticationException seja lançada.
        }

        [Test]
        public async Task LogoutAsync_Should_Revoke_Token_When_User_Exists()
        {
            // Arrange
            // - Mockar userRepository.GetByRefreshTokenAsync() para retornar um usuário válido.

            // Act
            // - Executar LogoutAsync().

            // Assert
            // - Verificar se user.RevokeRefreshToken() foi chamado.
            // - Verificar se userRepository.UpdateAsync() foi chamado.
            // - Validar que o retorno seja “Logout successful”.
        }

        [Test]
        public async Task LogoutAsync_Should_Return_Success_When_User_Not_Found()
        {
            // Arrange
            // - Mockar userRepository.GetByRefreshTokenAsync() para retornar null.

            // Act
            // - Executar LogoutAsync().

            // Assert
            // - Confirmar que o retorno seja “Logout successful”.
            // - Verificar que nenhum UpdateAsync() foi chamado.
        }
    }
}
