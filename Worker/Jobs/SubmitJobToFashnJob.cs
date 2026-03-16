using Application.DTOs.Job;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Infrastructure.Persistence;
using Shared.Interface;
using Worker.Models.Worker.Models;
using Worker.Services.Interface;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Worker.Jobs
{

    public class SubmitJobToFashnJob : ISubmitJob
    {
        private readonly AppDbContext context;
        private readonly IFashnAIService fashnAiService;
        private readonly ILogger<SubmitJobToFashnJob> logger;
        private readonly AppConfigurations settings;

        private JobExecution? execution = null;
        private Subscription? subscription = null;
        public SubmitJobToFashnJob(
            AppDbContext context,
            IFashnAIService fashnAiService,
            ILogger<SubmitJobToFashnJob> logger,
            IOptions<AppConfigurations> configurations
            )
        {
            this.context = context;
            this.fashnAiService = fashnAiService;
            this.logger = logger;
            this.settings = configurations.Value;
        }
        [JobDisplayName("SubmitJob")]
        public async Task ExecuteAsync(Guid jobExecutionId, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Starting job submission for JobExecution {ExecutionId}", jobExecutionId);

            try
            {
                execution = await GetPendingJobExecutionAsync(jobExecutionId, cancellationToken);

                subscription = await GetSubscriptionAsync(jobExecutionId, cancellationToken);

                var parameters = DeserializeJobParameters(execution);

                var medias = await GetJobMediasAsync(execution.JobId, cancellationToken);

                var request = BuildFashnRequest(parameters, medias);

                var response = await fashnAiService.SubmitJobAsync(request, cancellationToken);

                //Runtime Error
                if (response.Error != null)
                    throw new ExternalServiceException(response.Error.Name);

                execution.Start(response.Id);
                await context.SaveChangesAsync(cancellationToken);

                BackgroundJob.Schedule<PollJobStatusJob>(
                    job => job.ExecuteAsync(execution.Id, 1, CancellationToken.None),
                    TimeSpan.FromSeconds(settings.PollingIntervalSeconds));

                logger.LogInformation(
                    "JobExecution {ExecutionId} submitted successfully. External Job ID: {ExternalJobId}",
                    jobExecutionId, response.Id);
            }
            catch (ExternalServiceException ex)
            {
                await HandleJobFailureAsync(ex.Message, cancellationToken);
                throw;
            }
            catch (Exception ex)
            {
                await HandleJobFailureAsync(null, cancellationToken);
                throw;
            }
        }

        private async Task<JobExecution?> GetPendingJobExecutionAsync(Guid jobExecutionId, CancellationToken cancellationToken)
        {
            var execution = await context.JobExecution
                                    .Include(je => je.Job)
                                    .FirstOrDefaultAsync(je => je.Id == jobExecutionId, cancellationToken);

            if (execution == null)
                throw new InvalidOperationException("JobExecution não existe");
            if (execution.Status != JobStatus.Pending)
                throw new InvalidOperationException("Job em outra situação");

            logger.LogInformation("JobExecution validado {ExecutionId}", jobExecutionId);
            return execution;
        }

        private static JobParameters? DeserializeJobParameters(JobExecution execution)
        {
            try
            {
                return JsonSerializer.Deserialize<JobParameters>(execution.InputParameters);
            }
            catch
            {
                throw new InvalidOperationException("Não conseguiu desserializar inputs");
            }
        }

        private async Task<Subscription?> GetSubscriptionAsync(Guid jobExecutionId, CancellationToken cancellationToken)
        {
            var subscription = await context.JobExecution
                            .Where(je => je.Id == jobExecutionId)
                            .Select(je => je.Job.User.Subscription)
                            .FirstOrDefaultAsync(cancellationToken);

            if (subscription == null)
                throw new InvalidOperationException("Subscription não existe");

            return subscription;
        }

        private async Task<List<JobMedia>> GetJobMediasAsync(Guid jobId, CancellationToken cancellationToken)
        {
            var medias = await context.JobMedia
                .Where(jm => jm.JobId == jobId)
                .ToListAsync(cancellationToken);

            if (!medias.Any())
                throw new InvalidOperationException("Não encontrou as Midias");

            return medias;
        }

        private FashnRunRequest BuildFashnRequest(JobParameters parameters, List<JobMedia> medias)
        {
            var request = new FashnRunRequest
            {
                ModelName = parameters.Type,
                Inputs = new FashnInputs
                {
                    Category = GetEnumMemberValue(parameters.Category),
                    GarmentPhotoType = GetEnumMemberValue(parameters.GarmentPhotoType),
                    Mode = GetEnumMemberValue(parameters.Mode),
                    Seed = parameters.Seed,
                    NumSamples = parameters.NumSamples,
                    OutputFormat = "jpeg"
                }
            };

            switch (parameters.Type)
            {
                case JobType.TryOn:
                    request.Inputs.ModelImage = GetMediaUrl(medias, MediaCategory.Model);
                    request.Inputs.GarmentImage = GetMediaUrl(medias, MediaCategory.Garment);
                    break;

                case JobType.ModelCreating:
                    request.Inputs.GarmentImage = GetMediaUrl(medias, MediaCategory.Garment);
                    break;

                case JobType.ProductToModel:
                    request.Inputs.ModelImage = GetMediaUrl(medias, MediaCategory.Model);
                    request.Inputs.ProductImage = GetMediaUrl(medias, MediaCategory.Product);
                    break;

                case JobType.FaceToModel:
                    request.Inputs.ModelImage = GetMediaUrl(medias, MediaCategory.Model);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(parameters.Type), parameters.Type, "Tipo de job não suportado.");
            }

            return request;
        }

        private string GetMediaUrl(ICollection<JobMedia> medias, MediaCategory category)
        {
            return medias.FirstOrDefault(m => m.Category == category)?.Url ??
                throw new BusinessException($"Url vazia ou mídia não encontrada para a categoria {category}."); ;
        }


        private async Task HandleJobFailureAsync(string? error, CancellationToken cancellationToken)
        {

            if (execution == null)
                return;

            execution.Fail(error);

            if (subscription == null)
                return;

            subscription.ReleaseCredits(execution.UsedCredits);

            logger.LogError(error, "Falha no Job ");

            await context.SaveChangesAsync(cancellationToken);
        }

        private static readonly JsonSerializerOptions EnumJsonOptions = new()
        {
            Converters = { new JsonStringEnumConverter() }
        };

        public static string GetEnumMemberValue<TEnum>(TEnum enumValue)
            where TEnum : struct, Enum
        {
            return JsonSerializer.Serialize(enumValue, EnumJsonOptions).Trim('"');
        }
    }
}