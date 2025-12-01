using MarketplaceSystem.Application.Common.DTOs.Chat;
using MediatR;

namespace MarketplaceSystem.Application.Features.Chat.Commands.SendMessage
{
    public record SendMessageCommand(int SenderId, SendMessageRequest Request) : IRequest<MessageDto>;
}
