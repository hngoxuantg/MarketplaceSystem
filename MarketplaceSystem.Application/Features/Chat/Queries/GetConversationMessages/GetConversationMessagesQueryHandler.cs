using AutoMapper;
using MarketplaceSystem.Application.Common.DTOs.Chat;
using MarketplaceSystem.Application.Common.Interfaces.IExternalServices.IStorageServices;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Enums.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;
using System.Linq.Expressions;

namespace MarketplaceSystem.Application.Features.Chat.Queries.GetConversationMessages
{
    public class GetConversationMessagesQueryHandler : IRequestHandler<GetConversationMessagesQuery, List<MessageDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        public GetConversationMessagesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<List<MessageDto>> Handle(GetConversationMessagesQuery query, CancellationToken cancellation = default)
        {
            return await GetMessagesAsync(
                query.LastMessageId,
                query.LoadOlder,
                query.ConversationId,
                cancellation);
        }

        private async Task<List<MessageDto>> GetMessagesAsync(
            int? lastMessageId,
            bool loadOlder,
            int conversationId,
            CancellationToken cancellation = default)
        {
            Expression<Func<Message, bool>> filter;

            if (lastMessageId == null)
            {
                filter = m => true;
            }
            else if (loadOlder)
            {
                filter = m => m.Id < lastMessageId;
            }
            else
            {
                filter = m => m.Id > lastMessageId;
            }

            return (await _unitOfWork.MessageRepository.GetMessagesAsync<MessageDto>(
                conversationId: conversationId,
                filter: filter,
                take: 25,
                selector: m => new MessageDto
                {
                    Id = m.Id,
                    ConversationId = m.ConversationId,
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    Content = m.MessageType == MessageType.Text ? m.Content : null,
                    Type = m.MessageType,
                    SentAt = m.CreatedAt.ToLocalTime(),
                    AttachmentUrl = m.MessageType == MessageType.Image
                        ? _fileService.GetAbsoluteUrl(m.Content)
                        : null
                },
                cancellation: cancellation)).ToList();
        }
    }
}
