using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Repository
{
    public interface IJobRepository : IRepository<Job>
    {
        Task<(IEnumerable<Job> Items, int TotalCount)> GetPagedByUserAsync(
            Guid userId,
            int page,
            int pageSize,
            string? type = null,
            JobStatus? status = null,
            DateTime? from = null,
            DateTime? to = null);

        Task<Job?> GetByIdWithDetailsAsync(Guid jobId);
    }
}
