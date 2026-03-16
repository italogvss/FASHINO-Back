using Application.DTOs.Media;

namespace Application.Interfaces.Services
{
    public interface IMediaService
    {
        Task<MediaResponse> UploadMediaAsync(MediaUploadRequest request);
        Task<MediaListResponse> GetMediaListAsync(MediaRequest request, int page, int pageSize);
        Task<MediaResponse> GetMediaByIdAsync(Guid id);
        Task DeleteMediaAsync(Guid id);
        Task<MediaResponse> GetMediaByStorageIdAsync(string storageId);
        Task<List<MediaResponse>> GetMediaByIdsAsync(IEnumerable<Guid> ids);
    }
}
