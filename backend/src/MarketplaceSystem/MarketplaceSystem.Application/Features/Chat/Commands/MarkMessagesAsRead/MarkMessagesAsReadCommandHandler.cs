using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarketplaceSystem.Application.Features.Chat.Commands.MarkMessagesAsRead
{
    public class MarkMessagesAsReadCommandHandler : IRequestHandler<MarkMessagesAsReadCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        public MarkMessagesAsReadCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(MarkMessagesAsReadCommand command, CancellationToken cancellation = default)
        {
            return await MarkMessagesAsReadAsync(command.ConversationId, command.UserId, cancellation);
        }
        private async Task<int> MarkMessagesAsReadAsync(int conversationId, int userId, CancellationToken cancellation = default)
        {
            Conversation? conversion = await _unitOfWork.ConversationRepository.GetOneAsync<Conversation>(
                c => c.Id == conversationId,
                include: c => c.Include(c => c.Messages.Where(m => !m.IsRead)),
                cancellation: cancellation) ?? throw new NotFoundException("Conversation not found");

            conversion.MarkAllMessagesAsRead();

            await _unitOfWork.ConversationRepository.UpdateAsync(conversion, cancellation);

            if (conversion.UserBId != userId)
                return conversion.UserBId;
            return conversion.UserAId;
        }
    }
}
