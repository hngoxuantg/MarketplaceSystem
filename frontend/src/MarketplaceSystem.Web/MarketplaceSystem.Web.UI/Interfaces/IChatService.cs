using MarketplaceSystem.Web.UI.Models.ApiResponses;
using MarketplaceSystem.Web.UI.Models.ViewModels.Chat;

namespace MarketplaceSystem.Web.UI.Interfaces
{
    public interface IChatService
    {
        /// <summary>
        /// Get list of conversations with pagination
        /// </summary>
        Task<PaginatedResponse<ConversationViewModel>> GetConversationsAsync(int pageNumber = 1, int pageSize = 12);

        /// <summary>
        /// Get messages from a specific conversation
        /// </summary>
        Task<List<MessageViewModel>> GetConversationMessagesAsync(int conversationId, int? lastMessageId = null, bool loadOlder = true);

        /// <summary>
        /// Send a text message
        /// </summary>
        Task<MessageViewModel> SendMessageAsync(SendMessageViewModel model);

        /// <summary>
        /// Mark messages as read in a conversation
        /// </summary>
        Task<bool> MarkMessagesAsReadAsync(int conversationId);
    }
}
