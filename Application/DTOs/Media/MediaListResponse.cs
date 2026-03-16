namespace Application.DTOs.Media
{
    public record MediaListResponse(
        IEnumerable<MediaResponse> Items,
        int TotalCount,
        int Page,
        int PageSize,
        int TotalPages
    );
}
