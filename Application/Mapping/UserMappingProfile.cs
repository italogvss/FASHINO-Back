using Application.DTOs.Auth;
using Application.DTOs.User;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using FirebaseAdmin.Auth;

namespace Application.Mapping
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserDto>();

            CreateMap<User, UserInfo>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.DisplayName));

            CreateMap<UserRecord, AuthenticationUser>();
        }
    }
}
