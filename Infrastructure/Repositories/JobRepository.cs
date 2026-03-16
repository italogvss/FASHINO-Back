using Application.Interfaces.Repository;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class JobRepository : Repository<Job>, IJobRepository
    {
        public JobRepository(AppDbContext context) : base(context) { }

        public async Task<Job?> GetByIdWithDetailsAsync(Guid jobId)
        {
            return await _dbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(j => j.Medias)
                    .ThenInclude(jm => jm.Media)
                .Include(j => j.Executions)
                .Include(j => j.User)
                .FirstOrDefaultAsync(j => j.Id == jobId);
        }

        public async Task<(IEnumerable<Job> Items, int TotalCount)> GetPagedByUserAsync(
            Guid userId,
            int page,
            int pageSize,
            string? type = null,
            JobStatus? status = null,
            DateTime? from = null,
            DateTime? to = null)
        {
            var query = _dbSet
                .AsQueryable()
                .Include(j => j.Medias)
                .Include(j => j.Executions)
                .Where(j => j.UserId == userId);

            // 🔹 Filtro por tipo (string vem do front, precisa converter)
            if (!string.IsNullOrWhiteSpace(type) && Enum.TryParse<JobType>(type, true, out var parsedType))
                query = query.Where(j => j.Type == parsedType);

            // 🔹 Filtro por status
            if (status.HasValue)
                query = query.Where(j => j.Status == status.Value);

            // 🔹 Filtro por intervalo de datas
            if (from.HasValue)
                query = query.Where(j => j.CreatedAt >= from.Value.ToUniversalTime());

            if (to.HasValue)
                query = query.Where(j => j.CreatedAt <= to.Value.ToUniversalTime());

            // 🔹 Total antes da paginação
            var totalCount = await query.CountAsync();

            // 🔹 Paginação e ordenação
            var items = await query
                .OrderByDescending(j => j.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}

