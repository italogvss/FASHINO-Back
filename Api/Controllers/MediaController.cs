using Application.DTOs.Media;
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
    public class MediaController(IMediaService service) : ControllerBase
    {

        [HttpPost("upload")]
        [SwaggerOperation(
            Summary = "Upload Media",
            Description = "Envia uma nova mídia para o sistema e retorna os detalhes do arquivo enviado."
        )]
        public async Task<ApiResponse<MediaResponse>> Upload([FromForm] MediaUploadRequest request)
        {
            var userId = HttpContext.GetUserId() ??
                throw new UnauthorizedAccessException("User ID not found in token.");
            request.UserId = userId;
            var response = await service.UploadMediaAsync(request);
            return ApiResponse<MediaResponse>.SuccessResult(response, "Upload realizado com sucesso");
        }

        [HttpGet("list")]
        [SwaggerOperation(
            Summary = "List Media",
            Description = "Retorna uma lista paginada de mídias, filtrável por categoria e usuário."
        )]
        public async Task<ApiResponse<MediaListResponse>> List(
            [FromQuery] MediaRequest request,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var userId = HttpContext.GetUserId() ??
                throw new UnauthorizedAccessException("User ID not found in token.");
            request.UserId = userId;
            var response = await service.GetMediaListAsync(request, page, pageSize);
            return ApiResponse<MediaListResponse>.SuccessResult(response, "Lista de mídias obtida com sucesso");
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation(
            Summary = "Get Media by ID",
            Description = "Retorna os detalhes de uma mídia específica pelo seu ID."
        )]
        public async Task<ApiResponse<MediaResponse>> GetById(Guid id)
        {
            var response = await service.GetMediaByIdAsync(id);
            return ApiResponse<MediaResponse>.SuccessResult(response, "Mídia obtida com sucesso");
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation(
            Summary = "Delete Media",
            Description = "Deleta uma mídia específica. Somente o proprietário pode deletar."
        )]
        public async Task<ApiResponse<object>> Delete(Guid id)
        {
            await service.DeleteMediaAsync(id);
            return ApiResponse<object>.SuccessResult(null, "Mídia deletada com sucesso");
        }
    }
}
