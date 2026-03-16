using AutoMapper;
using Application.DTOs.Job;
using Application.DTOs.Media;
using Application.Interfaces.Persistence;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Shared.Interface;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Application.Services
{
    public class JobsService : IJobsService
    {
        private readonly IUnitOfWork UoW;
        private readonly IJobRepository jobRepository;
        private readonly IJobExecutionRepository executionRepository;
        private readonly IJobMediaRepository jobMediaRepository;
        private readonly IMediaRepository mediaRepository;
        private readonly ISubscriptionRepository subscriptionRepository;
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly IMediaService mediaService;
        private readonly ILogger<JobsService> log;
        private readonly AppConfigurations configuration;

        public JobsService(
            IJobRepository jobRepo,
            IJobExecutionRepository executionRepo,
            IJobMediaRepository jobMediaRepo,
            IMediaRepository mediaRepo,
            IMediaService mediaService,
            ISubscriptionRepository subscriptionRepository,
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<JobsService> log,
            IOptions<AppConfigurations> configMonitor,
            IUnitOfWork UoW)
        {
            this.jobRepository = jobRepo;
            this.executionRepository = executionRepo;
            this.jobMediaRepository = jobMediaRepo;
            this.mediaRepository = mediaRepo;
            this.subscriptionRepository = subscriptionRepository;
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.mediaService = mediaService;
            this.configuration = configMonitor.Value;
            this.UoW = UoW;
            this.log = log;
        }


        public async Task<JobDto?> GetJobById(Guid jobId)
        {
            var job = await jobRepository.GetByIdWithDetailsAsync(jobId);
            if (job == null)
                throw new NotFoundException("Job não encontrado");

            var jobDto = mapper.Map<JobDto>(job);

            var updatedMedias = new List<JobResponse>();
            if (job.Status == JobStatus.Completed)
                foreach (var media in job.Medias)
                {
                    var mediaResponse = await mediaService.GetMediaByIdAsync(media.MediaId);
                    updatedMedias.Add(new JobResponse { MediaId = media.MediaId, Url = mediaResponse.Url });
                }
            jobDto.Medias = updatedMedias;
            return jobDto;
        }

        public async Task<JobCreatedDto> CreateJob(CreateJobRequest dto)
        {
            if (!await userRepository.IsUserValid(dto.UserId))
                throw new NotFoundException("Usuário não encontrado");


            //Verifica se tem jobs pendentes
            if (await executionRepository.HasPendingJobsAsync(dto.UserId))
                throw new BusinessException("Usuário possui jobs pendentes. Aguarde a finalização antes de criar um novo job.");

            // Verifica créditos
            Subscription subscription = await subscriptionRepository.GetByUserId(dto.UserId)
                ?? throw new NotFoundException("Não achou inscrição");
            if (subscription == null)
                throw new NotFoundException("Inscrição não encontrada para o usuário informado.");

            var credits = dto.Parameters.PhotoPrice * dto.Parameters.NumSamples;

            if (subscription.HasAvailableCredits(credits))
                throw new BusinessException("Usuário não possui créditos disponíveis ou inscrição para criar o job.");

            var mediaList = await mediaService.GetMediaByIdsAsync(dto.Parameters.MediaIds);
            if (mediaList.Count == 0)
                throw new NotFoundException("Nenhuma mídia encontrada para os IDs fornecidos.");


            if (!IsMediaValid(dto, mediaList))
                throw new BusinessException("Mídias inválidas para o tipo de job solicitado.");

            // Inicia Unit of Work (transação)
            await UoW.BeginTransactionAsync();

            try
            {

                // Cria Job
                var job = new Job(dto.Type, dto.UserId);
                await jobRepository.AddAsync(job);

                // Cria execução inicial
                var inputParametersJson = JsonSerializer.Serialize<JobParameters>(dto.Parameters);

                var execution = new JobExecution(job.Id, inputParametersJson, configuration.MaxJobRetry, credits);
                await executionRepository.AddAsync(execution);
                job.AddExecution(execution);

                // Associa mídias ao Job
                foreach (var media in mediaList)
                {
                    var jm = new JobMedia(job.Id, media.Id, execution.Id, media.Url, media.Category);
                    await jobMediaRepository.AddAsync(jm);
                    job.AddMedia(jm);
                }

                // Pega inscrição para consumir crédito
                subscription.AllocateCredits(credits);

                await subscriptionRepository.UpdateAsync(subscription);
                // Salva todas as alterações e commit

                await UoW.CommitAsync();

                // Enfileira o job no Hangfire para processamento assíncrono
                BackgroundJob.Enqueue<ISubmitJob>(job => job.ExecuteAsync(execution.Id, CancellationToken.None));

                return new JobCreatedDto
                (
                    job.Id,
                    job.Status,
                    job.CreatedAt
                );
            }
            catch
            {
                await UoW.RollbackAsync();
                throw new BusinessException("Algo não deu certo");
            }
        }

        private bool IsMediaValid(CreateJobRequest job, List<MediaResponse> mediaList)
        {
            switch (job.Type)
            {
                case JobType.TryOn:
                    return mediaList.Exists(m => m.Category == MediaCategory.Garment) &&
                        mediaList.Exists(m => m.Category == MediaCategory.Model);
                case JobType.ModelCreating:
                    return mediaList.Exists(m => m.Category == MediaCategory.Model);
                case JobType.ProductToModel:
                    return mediaList.Exists(m => m.Category == MediaCategory.Garment) &&
                        mediaList.Exists(m => m.Category == MediaCategory.Model);
                case JobType.FaceToModel:
                    return mediaList.Exists(m => m.Category == MediaCategory.Face);
                default:
                    throw new BusinessException("Tipo de job inválido.");
            }
        }

        public async Task<JobReprocessDto> ReprocessJob(Guid jobId)
        {
            // 🔹 Busca job com execuções
            var job = await jobRepository.GetByIdWithDetailsAsync(jobId);
            if (job is null)
                throw new NotFoundException("Job não encontrado.");

            // 🔹 Pega última execução
            var lastExec = job.Executions?
                .OrderByDescending(e => e.CreatedAt)
                .FirstOrDefault();

            if (lastExec is null)
                throw new BusinessException("Job não possui execuções para reprocessar.");

            if (lastExec.Status == JobStatus.Processing)
                throw new BusinessException("Execução em processamento, não é possível reprocessar agora.");

            // ✅ Inicia transação
            await UoW.BeginTransactionAsync();

            try
            {
                // 🔹 Cria nova execução baseada na última
                var newExec = new JobExecution(job.Id, lastExec.InputParameters, lastExec.MaxRetries);
                await executionRepository.AddAsync(newExec);
                job.AddExecution(newExec);

                // 🔹 Persiste alterações
                await UoW.CommitAsync();

                var subscription = await subscriptionRepository.GetByUserId(job.UserId)
                ?? throw new NotFoundException("Não achou inscrição");

                // Enfileira o job no Hangfire para processamento assíncrono
                BackgroundJob.Enqueue<ISubmitJob>(job => job.ExecuteAsync(newExec.Id, CancellationToken.None));

                return new JobReprocessDto(
                    job.Id,
                    newExec.Id,
                    job.Status,
                    DateTime.UtcNow
                );
            }
            catch
            {
                await UoW.RollbackAsync();
                throw;
            }
        }
    }
}
