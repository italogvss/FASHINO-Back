using Application.DTOs.User;

namespace Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(Guid id);
        Task<UserDto> GetUserByFirebaseUidAsync(string firebaseUid);
        Task<UserDto> UpdateUserAsync(Guid id, UserDto updateUserDto);
    }
}
