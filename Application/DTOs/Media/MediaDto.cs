using Domain.Enums;

namespace Application.DTOs.Media
{
    public record MediaDto(
        Guid Id,
        string ObjectKey,
        string MimeType,
        long Size,
        string? FileName,
        MediaCategory Category,
        Guid? UserId,
        DateTime CreatedAt
    );
}
