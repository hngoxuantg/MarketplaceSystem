using AutoMapper;
using MarketplaceSystem.Application.Common.DTOs.Chat;
using MarketplaceSystem.Application.Common.Interfaces.IExternalServices.IStorageServices;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Enums.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;

namespace MarketplaceSystem.Application.Features.Chat.Commands.SendMessage
{
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, MessageDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public SendMessageCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<MessageDto> Handle(SendMessageCommand command, CancellationToken cancellation = default)
        {
            return await SendMessageAsync(command.SenderId, command.Request, cancellation);
        }
        private async Task<MessageDto> SendMessageAsync(int senderId, SendMessageRequest request, CancellationToken cancellation = default)
        {
            ValidateRequest(request);

            Conversation conversation = await GetOrCreateConversationAsync(senderId, request, cancellation);

            Message message = await CreateMessageAsync(senderId, request, cancellation);

            await _unitOfWork.BeginTransactionAsync(cancellation);
            try
            {
                if (conversation.Id == 0)
                {
                    conversation.AddMessage(message);
                    await _unitOfWork.ConversationRepository.CreateAsync(conversation, cancellation);
                }
                else
                {
                    conversation.AddMessage(message);
                    await _unitOfWork.ConversationRepository.UpdateAsync(conversation, cancellation);
                }

                Conversation newConversation = await GetOrCreateConversationAsync(senderId, request, cancellation);

                newConversation.SetLastMessage(message.Id);
                await _unitOfWork.ConversationRepository.UpdateAsync(newConversation, cancellation);

                await _unitOfWork.CommitTransactionAsync(cancellation);

                return MapToDto(message);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellation);
                throw;
            }

        }

        private void ValidateRequest(SendMessageRequest request)
        {
            if (request.Image == null && string.IsNullOrWhiteSpace(request.Content))
                throw new Exception("Phải có nội dung hoặc ảnh");
            if (request.Image != null && !string.IsNullOrWhiteSpace(request.Content))
                throw new Exception("Chỉ được gửi nội dung hoặc ảnh");
        }

        private async Task<Conversation> GetOrCreateConversationAsync(int senderId, SendMessageRequest request, CancellationToken cancellation)
        {
            Conversation? conversation = await _unitOfWork.ConversationRepository.GetOneAsync<Conversation>(
                c => (c.UserAId == senderId && c.UserBId == request.ReceiverId) ||
                     (c.UserAId == request.ReceiverId && c.UserBId == senderId),
                cancellation: cancellation);

            if (conversation == null)
                conversation = new Conversation
                {
                    UserAId = senderId,
                    UserBId = request.ReceiverId
                };

            return conversation;
        }

        private async Task<Message> CreateMessageAsync(int senderId, SendMessageRequest request, CancellationToken cancellation)
        {
            Message message = new Message(senderId, request.ReceiverId);

            if (!string.IsNullOrWhiteSpace(request.Content))
            {
                message.SetContent(request.Content);
                message.SetType(MessageType.Text);
            }
            else
            {
                message.SetType(MessageType.Image);
                message.SetContent(await _fileService.SaveImageAsync(request.Image!, "chat_images", cancellation));
            }

            return message;
        }

        private MessageDto MapToDto(Message message)
        {
            MessageDto dto = _mapper.Map<MessageDto>(message);

            if (message.MessageType == MessageType.Image)
            {
                dto.AttachmentUrl = _fileService.GetAbsoluteUrl(message.Content);
                dto.Content = null;
            }
            else
            {
                dto.AttachmentUrl = null;
                dto.Content = message.Content;
            }

            return dto;
        }

    }
}
