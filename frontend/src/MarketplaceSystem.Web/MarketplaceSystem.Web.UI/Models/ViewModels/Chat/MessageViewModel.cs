namespace MarketplaceSystem.Web.UI.Models.ViewModels.Chat
{
    public class MessageViewModel
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string? Content { get; set; }
        public string? AttachmentUrl { get; set; }
        public string Type { get; set; } // "Text", "Image"
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
        
        // Helper properties for UI
        public bool IsOwnMessage { get; set; }
        public string SenderName { get; set; }
        public string TimeAgo { get; set; }
    }
}
