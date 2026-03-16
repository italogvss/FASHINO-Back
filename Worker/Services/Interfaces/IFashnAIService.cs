using Sahared.Common;
using Worker.Models.Worker.Models;
using Microsoft.AspNetCore.Http;

namespace Worker.Services.Interface
{
    public interface IFashnAIService
    {
        Task<FashnRunResponse> SubmitJobAsync(FashnRunRequest request, CancellationToken cancellationToken = default);
        Task<FashnRunResponse> GetStatusAsync(string externalJobId, CancellationToken cancellationToken = default);
        Task<IFormFile> DownloadImageAsync(string firstImageUrl, CancellationToken cancellationToken);
    }
}
