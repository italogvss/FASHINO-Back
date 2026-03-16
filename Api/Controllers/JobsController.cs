using Application.DTOs.Job;
using Application.Interfaces.Services;
using Domain.Common;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class JobsController(IJobsService service) : ControllerBase
    {

        [HttpGet("{id:guid}")]
        [SwaggerOperation(
            Summary = "Get job by ID",
            Description = "Returns a single job with details, status, and result URLs.")]
        public async Task<ApiResponse<JobDto?>> GetJobById([FromRoute] Guid id)
        {
            var response = await service.GetJobById(id);
            return ApiResponse<JobDto?>.SuccessResult(response);
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Create a new try-on job",
            Description = "Creates a new try-on job with specified media and options.")]
        public async Task<ApiResponse<JobCreatedDto>> CreateJob([FromBody] CreateJobRequest request)
        {
            var userId = HttpContext.GetUserId() ??
                    throw new UnauthorizedAccessException("User ID not found in token.");
            request.UserId = userId;
            var response = await service.CreateJob(request);
            return ApiResponse<JobCreatedDto>.SuccessResult(response);
        }

        [HttpPatch("{id:guid}/reprocess")]
        [SwaggerOperation(
            Summary = "Reprocess job",
            Description = "Reprocesses a failed job.")]
        public async Task<ApiResponse<JobReprocessDto>> ReprocessJob([FromRoute] Guid id)
        {
            var response = await service.ReprocessJob(id);
            return ApiResponse<JobReprocessDto>.SuccessResult(response);
        }
    }
}
