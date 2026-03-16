using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }


        public async Task<User?> GetByFirebaseUidAsync(string firebaseUid)
        {
            return await _dbSet
                .Include(u => u.Subscription)
                .FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);
        }

        public Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return null;

            return _dbSet.
                FirstOrDefaultAsync(u => u.RefreshToken != null && u.RefreshToken == refreshToken);
        }

        public async Task<bool> IsUserValid(Guid id)
        {
            return await _dbSet
                .AnyAsync(u => u.Id == id && u.IsActive == true);
        }
    }
}
