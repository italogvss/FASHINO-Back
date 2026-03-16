using Application.DTOs.Job;

namespace Application.Interfaces.Services
{
    public interface IJobsService
    {
        Task<JobDto?> GetJobById(Guid jobId);
        Task<JobCreatedDto> CreateJob(CreateJobRequest dto);
        Task<JobReprocessDto> ReprocessJob(Guid jobId);
    }
}
