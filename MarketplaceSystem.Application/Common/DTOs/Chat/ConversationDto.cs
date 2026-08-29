using MarketplaceSystem.Domain.Enums.Business;

namespace MarketplaceSystem.Application.Common.DTOs.Chat
{
    public class ConversationDto
    {
        public int ConversationId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string? LastMessage { get; set; }
        public MessageType LastMessageType { get; set; }
        public DateTime LastMessageAt { get; set; }
        public int UnreadCount { get; set; }
    }
}
