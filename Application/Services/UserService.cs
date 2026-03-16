using AutoMapper;
using Application.DTOs.User;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;

namespace Application.Services
{

    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;

        }

        public Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> GetUserByFirebaseUidAsync(string firebaseUid)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> GetUserByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> UpdateUserAsync(Guid id, UserDto updateUserDto)
        {
            throw new NotImplementedException();
        }
    }

}
