using MarketplaceSystem.Application.Common.DTOs.Chat;
using MarketplaceSystem.Common.Models.Pagination;
using MediatR;

namespace MarketplaceSystem.Application.Features.Chat.Queries.GetConversations
{
    public record GetConversationsQuery(int UserId, GetConversationsRequest Request) : IRequest<PaginatedResult<ConversationDto>>;
}
