using MarketplaceSystem.Application.Common.DTOs.Chat;
using MarketplaceSystem.Common.Models.Pagination;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;
using System.Linq.Expressions;

namespace MarketplaceSystem.Application.Features.Chat.Queries.GetConversations
{
    public class GetConversationsQueryHanlder : IRequestHandler<GetConversationsQuery, PaginatedResult<ConversationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetConversationsQueryHanlder(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResult<ConversationDto>> Handle(GetConversationsQuery query, CancellationToken cancellation = default)
        {
            return await GetConversationsAsync(query.UserId, query.Request, cancellation);
        }
        private async Task<PaginatedResult<ConversationDto>> GetConversationsAsync(
            int userId,
            PaginatedRequest request,
            CancellationToken cancellation = default)
        {
            Expression<Func<Conversation, bool>> filter = c =>
                (c.UserAId == userId || c.UserBId == userId);

            (IEnumerable<ConversationDto> conversations, int totalCount) = await _unitOfWork.ConversationRepository.GetPagedAsync(
                filter: filter,
                orderBy: q => q.OrderByDescending(c => c.Message != null ? c.Message.CreatedAt : DateTime.MinValue),
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                selector: c => new ConversationDto
                {
                    ConversationId = c.Id,
                    UserId = c.UserAId == userId ? c.UserBId : c.UserAId,
                    UserName = c.UserAId == userId ? c.UserB!.Profile.FullName : c.User!.Profile.FullName,
                    LastMessage = c.Message != null ? c.Message.Content : null,
                    LastMessageType = c.Message.MessageType,
                    LastMessageAt = c.Message != null ? c.Message.CreatedAt : DateTime.MinValue,
                    UnreadCount = c.Messages != null ? c.Messages.Count(m => !m.IsRead && m.SenderId != userId) : 0
                },
                cancellationToken: cancellation
            );

            return new PaginatedResult<ConversationDto>(
                data: conversations,
                totalCount: totalCount,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize
            );
        }
    }
}
