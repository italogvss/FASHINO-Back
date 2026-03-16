namespace Application.DTOs.Job
{
    public record JobListResponse
    (
        IEnumerable<JobResponse> Items,
        int TotalCount,
        int Page,
        int PageSize,
        int TotalPages
    );
}
