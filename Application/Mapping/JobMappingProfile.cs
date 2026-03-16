using AutoMapper;
using Application.DTOs.Job;
using Domain.Entities;

namespace Application.Mapping
{
    public class JobMappingProfile : Profile
    {
        public JobMappingProfile()
        {
            // Job -> JobDto
            CreateMap<Job, JobDto>()
                .ForMember(dest => dest.Type,
                    opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Medias,
                    opt => opt.MapFrom(src => src.Medias))
                .ForMember(dest => dest.Executions,
                    opt => opt.MapFrom(src => src.Executions));

            // JobMedia -> JobMediaDto
            CreateMap<JobMedia, JobResponse>()
                .ForMember(dest => dest.MediaId,
                    opt => opt.MapFrom(src => src.MediaId))
                .ForMember(dest => dest.Url,
                    opt => opt.MapFrom(src => src.Url))
                .ForMember(dest => dest.Category,
                    opt => opt.MapFrom(src => src.Category));

            // JobMedia -> JobResponse (versão simplificada)
            CreateMap<JobMedia, JobResponse>()
                .ForMember(dest => dest.MediaId,
                    opt => opt.MapFrom(src => src.MediaId))
                .ForMember(dest => dest.Url,
                    opt => opt.MapFrom(src => src.Url))
                .ForMember(dest => dest.Category,
                    opt => opt.MapFrom(src => src.Category));

            // JobExecution -> JobExecutionDto
            CreateMap<JobExecution, JobExecutionDto>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ExternalJobId,
                    opt => opt.MapFrom(src => src.ExternalJobId))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.ErrorMessage,
                    opt => opt.MapFrom(src => src.ErrorMessage))
                .ForMember(dest => dest.UsedCredits,
                    opt => opt.MapFrom(src => src.UsedCredits))
                .ForMember(dest => dest.RetryCount,
                    opt => opt.MapFrom(src => src.RetryCount))
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.StartedAt,
                    opt => opt.MapFrom(src => src.StartedAt))
                .ForMember(dest => dest.CompletedAt,
                    opt => opt.MapFrom(src => src.CompletedAt));
        }
    }
}
