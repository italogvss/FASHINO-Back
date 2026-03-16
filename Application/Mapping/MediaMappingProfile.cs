using AutoMapper;
using Application.DTOs.Media;
using Domain.Entities;

namespace Application.Mapping
{
    public class MediaMappingProfile : Profile
    {
        public MediaMappingProfile()
        {
            CreateMap<Media, MediaDto>();

            CreateMap<Media, MediaResponse>()
                .ForMember(dest => dest.Url, opt => opt.Ignore());
        }
    }
}
