using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface IJobMediaRepository : IRepository<JobMedia>
    {
        Task<IEnumerable<JobMedia>> GetByJobIdAsync(Guid jobId);
    }
}
