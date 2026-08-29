using AutoMapper;
using MarketplaceSystem.Application.Common.DTOs.Users;
using MarketplaceSystem.Domain.Entities.Identity_Auth;

namespace MarketplaceSystem.Application.Common.Mappers
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(u => u.Id,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(u => u.FullName,
                opt => opt.MapFrom(src => src.Profile.FullName))
                .ForMember(u => u.Avatar,
                opt => opt.MapFrom(src => src.Profile.Avatar))
                .ForMember(u => u.Email,
                opt => opt.MapFrom(src => src.Email));

            CreateMap<User, SellerDto>()
                .ForMember(u => u.UserId,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(u => u.DisplayName,
                opt => opt.MapFrom(src => src.Profile.FullName))
                .ForMember(u => u.AvatarUrl,
                opt => opt.MapFrom(src => src.Profile.Avatar))
                .ForMember(u => u.PhoneNumber,
                opt => opt.MapFrom(src => src.Profile.ShowPhoneNumber));
        }
    }
}
