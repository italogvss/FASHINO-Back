using AutoMapper;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using Application.Services;
using Domain.Common;
using Microsoft.Extensions.Options;
using Moq;

namespace Tests.Commons
{
    public abstract class TestFixtureBase
    {
        protected Mock<IUserRepository> UserRepositoryMock = new();
        protected Mock<IAppTokenService> TokenServiceMock = new();
        protected Mock<IMapper> MapperMock = new();
        protected Mock<IOptions<TokenSettings>> TokenSettingsMock = new();

        [SetUp]
        public virtual void SetupBase()
        {
        }

        protected AuthAppService CreateSut(IAuthService authService)
        {
            return new AuthAppService(
                authService,
                UserRepositoryMock.Object,
                MapperMock.Object,
                TokenServiceMock.Object
            );
        }
    }
}