using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Repository
{
    public interface IMediaRepository : IRepository<Media>
    {
        Task<Media?> GetByStorageIdAsync(string storageId);
        Task<(IEnumerable<Media> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            MediaCategory? category = null,
            Guid? userId = null,
            DateTime? startDate = null,
            DateTime? endDate = null);
    }
}
