using AutoMapper;
using MarketplaceSystem.Application.Common.DTOs.Chat;
using MarketplaceSystem.Domain.Entities.Business;

namespace MarketplaceSystem.Application.Common.Mappers
{
    public class ChatProfile : Profile
    {
        public ChatProfile()
        {
            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ConversationId,
                opt => opt.MapFrom(src => src.ConversationId))
                .ForMember(dest => dest.SenderId,
                opt => opt.MapFrom(src => src.SenderId))
                .ForMember(dest => dest.ReceiverId,
                opt => opt.MapFrom(src => src.ReceiverId))
                .ForMember(dest => dest.Content,
                opt => opt.Ignore())
                .ForMember(dest => dest.Type,
                opt => opt.Ignore())
                .ForMember(dest => dest.SentAt,
                opt => opt.MapFrom(src => src.CreatedAt));

        }
    }
}
