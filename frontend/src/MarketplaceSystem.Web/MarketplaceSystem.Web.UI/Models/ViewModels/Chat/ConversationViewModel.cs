namespace MarketplaceSystem.Web.UI.Models.ViewModels.Chat
{
    public class ConversationViewModel
    {
        public int ConversationId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string? UserAvatar { get; set; }
        public string? LastMessage { get; set; }
        public string LastMessageType { get; set; }
        public DateTime LastMessageAt { get; set; }
        public int UnreadCount { get; set; }
        
        // Helper properties
        public string TimeAgo { get; set; }
        public bool IsOnline { get; set; }
    }
}
