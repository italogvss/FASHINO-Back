using Application.Interfaces.Repository;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class MediaRepository : Repository<Media>, IMediaRepository
    {
        public MediaRepository(AppDbContext context) : base(context) { }

        public Task<Media?> GetByStorageIdAsync(string storageId)
        {
            return _dbSet.AsQueryable()
                .Where(m => m.ObjectKey == storageId)
                .FirstOrDefaultAsync();

        }

        public async Task<(IEnumerable<Media> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            MediaCategory? category = null,
            Guid? userId = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var query = _dbSet.AsQueryable();

            if (category.HasValue)
                query = query.Where(m => m.Category == category.Value);

            if (userId.HasValue)
                query = query.Where(m => m.UserId == userId.Value);

            if (startDate.HasValue)
                query = query.Where(m => m.CreatedAt >= startDate.Value.ToUniversalTime());

            if (endDate.HasValue)
                query = query.Where(m => m.CreatedAt <= endDate.Value.ToUniversalTime());

            var result = await query
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => new { Media = m, TotalCount = query.Count() }) // inclui count no mesmo select
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = result.Select(r => r.Media).ToList();
            var totalCount = result.FirstOrDefault()?.TotalCount ?? 0;

            return (items, totalCount);
        }

    }

}
