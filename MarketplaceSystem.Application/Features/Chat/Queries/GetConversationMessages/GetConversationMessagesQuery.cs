using MarketplaceSystem.Application.Common.DTOs.Chat;
using MediatR;

namespace MarketplaceSystem.Application.Features.Chat.Queries.GetConversationMessages
{
    public record GetConversationMessagesQuery(int ConversationId, int? LastMessageId, bool LoadOlder) : IRequest<List<MessageDto>>;
}
