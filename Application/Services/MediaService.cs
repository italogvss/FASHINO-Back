using AutoMapper;
using Application.DTOs.Media;
using Application.Interfaces.Persistence;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Services
{
    public class MediaService : IMediaService
    {
        private readonly IMediaRepository mediaRepository;
        private readonly IStorageService storageService;
        private readonly IUserRepository userRepository;
        private readonly ILogger<MediaService> logger;
        private readonly IMapper mapper;
        private readonly IOptions<MinIOSettings> minIoSettings;
        private readonly AppConfigurations configurations;
        private readonly IUnitOfWork unit;

        public MediaService(IMediaRepository mediaRepository,
                              IStorageService storageService,
                              IUserRepository userRepository,
                              ILogger<MediaService> logger,
                              IMapper mapper,
                              IOptions<MinIOSettings> settings,
                              IOptions<AppConfigurations> configMonitor,
                              IUnitOfWork unitOfWork)
        {
            this.mediaRepository = mediaRepository;
            this.storageService = storageService;
            this.userRepository = userRepository;
            this.logger = logger;
            this.mapper = mapper;
            this.minIoSettings = settings;
            this.configurations = configMonitor.Value;
            this.unit = unitOfWork;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public async Task<MediaResponse> UploadMediaAsync(MediaUploadRequest request)
        {
            ValidateUploadRequest(request);

            if (!await userRepository.IsUserValid(request.UserId))
                throw new NotFoundException("Usuário não encontrado");
            await unit.BeginTransactionAsync();

            string uploadedObjectKey = string.Empty;

            try
            {
                var folder = request.Category.ToString();
                var extension = Path.GetExtension(request.File.FileName);
                var finalFileName = $"{Guid.NewGuid()}{extension}";

                using (var stream = request.File.OpenReadStream())
                {
                    uploadedObjectKey = await storageService.UploadFileAsync(
                        stream,
                        finalFileName,
                        request.File.ContentType,
                        folder);
                }

                var media = new Media(
                    uploadedObjectKey,
                    request.File.ContentType,
                    request.File.Length,
                    request.Category,
                    finalFileName,
                    request.UserId);

                await mediaRepository.AddAsync(media);


                logger.LogInformation("Media uploaded successfully: {ObjectKey} for user {UserId}", uploadedObjectKey, request.UserId);

                var preSignedUrl = await storageService.GetPreSignedUrlAsync(media.ObjectKey, TimeSpan.FromHours(1));
                var response = mapper.Map<MediaResponse>(media);
                response.Url = preSignedUrl;

                await unit.CommitAsync();

                return mapper.Map<MediaResponse>(response);

            }
            catch (Exception e)
            {
                if (!string.IsNullOrEmpty(uploadedObjectKey))
                    await storageService.DeleteFileAsync(uploadedObjectKey);

                await unit.RollbackAsync();
                throw;
            }
        }

        public async Task<MediaListResponse> GetMediaListAsync(MediaRequest request, int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 1;


            if (!await userRepository.IsUserValid(request.UserId))
                throw new NotFoundException("Usuário não encontrado");

            var (items, totalCount) = await mediaRepository.GetPagedAsync(
                page,
                pageSize,
                request.Category,
                request.UserId,
                request.StartDate,
                request.EndDate);

            var mediaResponses = new List<MediaResponse>();
            foreach (var media in items)
            {
                var preSignedUrl = await storageService.GetPreSignedUrlAsync(media.ObjectKey, TimeSpan.FromHours(1));
                var response = mapper.Map<MediaResponse>(media);
                response.Url = preSignedUrl;
                mediaResponses.Add(response);
            }

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return new MediaListResponse(
                mediaResponses,
                totalCount,
                page,
                pageSize,
                totalPages);

        }

        public async Task<MediaResponse> GetMediaByIdAsync(Guid id)
        {
            var media = await mediaRepository.GetByIdAsync(id);
            if (media == null)
                throw new NotFoundException("Mídia não encontrada");

            var preSignedUrl = await storageService.GetPreSignedUrlAsync(media.ObjectKey, TimeSpan.FromHours(1));
            preSignedUrl = preSignedUrl.Replace("http://minio:9000", minIoSettings.Value.ExternalUrl);
            if (string.IsNullOrWhiteSpace(preSignedUrl))
                throw new ExternalServiceException("Error while generating pre-signed URL for media");

            var response = mapper.Map<MediaResponse>(media);
            response.Url = preSignedUrl;
            return response;
        }

        public async Task<List<MediaResponse>> GetMediaByIdsAsync(IEnumerable<Guid> ids)
        {
            var mediaList = new List<MediaResponse>();

            foreach (var id in ids)
            {
                var media = await mediaRepository.GetByIdAsync(id);
                if (media == null)
                    throw new NotFoundException($"Mídia com ID {id} não encontrada.");

                var preSignedUrl = await storageService.GetPreSignedUrlAsync(media.ObjectKey, TimeSpan.FromHours(1));
                if (string.IsNullOrWhiteSpace(preSignedUrl))
                    throw new ExternalServiceException($"Erro ao gerar URL pré-assinada para a mídia {id}.");

                // Ajusta URL externa
                preSignedUrl = preSignedUrl.Replace("http://minio:9000", minIoSettings.Value.ExternalUrl);

                var response = mapper.Map<MediaResponse>(media);
                response.Url = preSignedUrl;

                mediaList.Add(response);
            }

            return mediaList;
        }

        public async Task DeleteMediaAsync(Guid id)
        {
            var media = await mediaRepository.GetByIdAsync(id);
            if (media == null)
                throw new NotFoundException("Mídia não encontrada");

            await storageService.DeleteFileAsync(media.ObjectKey);
            await mediaRepository.DeleteAsync(media);
            await mediaRepository.SaveChangesAsync();
            logger.LogInformation("Media deleted successfully: {ObjectKey}", media.ObjectKey);

        }

        private void ValidateUploadRequest(MediaUploadRequest request)
        {
            var errors = new List<string>();

            if (request.File == null || request.File.Length == 0)
            {
                errors.Add("Arquivo é obrigatório.");
                throw new ValidationException("Erro Upload", errors);
            }

            var maxFileSize = configurations.MaxFileSizeMB * 1024L * 1024L;
            var allowedMimeTypes = configurations.AllowedMimes.Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim().ToLower()).ToArray();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

            var extension = Path.GetExtension(request.File.FileName)?.ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                errors.Add("Tipo de arquivo não permitido. Permitidos: JPG, PNG.");

            if (request.File.Length > maxFileSize)
                errors.Add($"Arquivo muito grande. Máximo permitido: {configurations.MaxFileSizeMB}MB.");

            if (request.File.ContentType == null || !allowedMimeTypes.Contains(request.File.ContentType.ToLower()))
                errors.Add("Tipo de conteúdo inválido.");

            if (request.Category == MediaCategory.Generated)
                errors.Add("Não é possível fazer upload de imagens geradas.");

            if (errors.Any())
                throw new ValidationException("Erro Upload", errors);
        }

        public async Task<MediaResponse> GetMediaByStorageIdAsync(string storageId)
        {
            var media = await mediaRepository.GetByStorageIdAsync(storageId);
            if (media == null)
                throw new NotFoundException("Mídia não encontrada");

            var preSignedUrl = await storageService.GetPreSignedUrlAsync(media.ObjectKey, TimeSpan.FromHours(1));
            preSignedUrl = preSignedUrl.Replace("http://minio:9000", minIoSettings.Value.ExternalUrl);
            if (string.IsNullOrWhiteSpace(preSignedUrl))
                throw new ExternalServiceException("Error while generating pre-signed URL for media");

            var response = mapper.Map<MediaResponse>(media);
            response.Url = preSignedUrl;
            return response;
        }

    }
}
