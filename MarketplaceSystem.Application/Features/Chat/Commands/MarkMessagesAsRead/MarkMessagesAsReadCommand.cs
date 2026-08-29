using MediatR;

namespace MarketplaceSystem.Application.Features.Chat.Commands.MarkMessagesAsRead
{
    public record MarkMessagesAsReadCommand(int UserId, int ConversationId) : IRequest<int>;
}
