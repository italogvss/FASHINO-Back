using AutoMapper;
using Application.DTOs.Subscription;
using Domain.Entities;

namespace Application.Mapping
{
    public class SubscriptionMappingProfile : Profile
    {
        public SubscriptionMappingProfile()
        {
            // Entity -> DTO
            CreateMap<Subscription, SubscriptionDto>();
            CreateMap<Subscription, SubscriptionResponse>();
        }
    }
}