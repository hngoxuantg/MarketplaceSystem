using MarketplaceSystem.Domain.Enums.Business;

namespace MarketplaceSystem.Application.Common.DTOs.Chat
{
    public class MessageDto
    {
        public int Id { get; set; }

        public int ConversationId { get; set; }

        public int SenderId { get; set; }

        public int ReceiverId { get; set; }

        public string? Content { get; set; }

        public string? AttachmentUrl { get; set; }

        public MessageType Type { get; set; }

        public DateTime SentAt { get; set; }
    }
}
