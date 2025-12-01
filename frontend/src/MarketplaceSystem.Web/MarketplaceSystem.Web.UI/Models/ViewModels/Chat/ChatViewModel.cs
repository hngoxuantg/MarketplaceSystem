namespace MarketplaceSystem.Web.UI.Models.ViewModels.Chat
{
    public class ChatViewModel
    {
        public List<ConversationViewModel> Conversations { get; set; } = new();
        public int CurrentConversationId { get; set; }
        public int CurrentUserId { get; set; }
        public string CurrentUserName { get; set; }
        public List<MessageViewModel> Messages { get; set; } = new();
        
        // Pagination
        public int TotalCount { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
}
