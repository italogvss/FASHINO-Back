using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByFirebaseUidAsync(string firebaseUid);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);
        Task<bool> IsUserValid(Guid id);
    }
}
