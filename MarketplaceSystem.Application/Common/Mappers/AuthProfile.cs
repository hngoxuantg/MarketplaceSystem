using AutoMapper;
using MarketplaceSystem.Application.Features.Auth.Commands.Register;
using MarketplaceSystem.Domain.Entities.Identity_Auth;

namespace MarketplaceSystem.Application.Common.Mappers
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<RegisterRequest, User>()
                .ForMember(dest => dest.UserName,
                opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Email,
                opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.LockoutEnabled,
                opt => opt.MapFrom(src => true));

            CreateMap<RegisterRequest, Domain.Entities.Business.UserProfile>()
                .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Location,
                opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.DateOfBirth,
                opt => opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.Gender,
                opt => opt.MapFrom(src => src.Gender));
        }
    }
}
