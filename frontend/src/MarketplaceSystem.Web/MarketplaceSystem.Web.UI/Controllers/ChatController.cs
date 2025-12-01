using MarketplaceSystem.Web.UI.Interfaces;
using MarketplaceSystem.Web.UI.Models.ViewModels.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceSystem.Web.UI.Controllers
{
    [Authorize]
    public class ChatController : BaseController
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        /// <summary>
        /// Chat main page - List all conversations
        /// </summary>
        [HttpGet]
        public IActionResult Index()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Don't load conversations here - let SignalR handle it
            var viewModel = new ChatViewModel
            {
                Conversations = new List<ConversationViewModel>(),
                CurrentUserId = userId.Value,
                TotalCount = 0,
                PageNumber = 1,
                PageSize = 20,
                TotalPages = 0,
                HasNextPage = false,
                HasPreviousPage = false
            };

            return View(viewModel);
        }

        /// <summary>
        /// Get conversation messages (AJAX/API endpoint for SignalR fallback)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetMessages(int conversationId, int? lastMessageId = null, bool loadOlder = true)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var messages = await _chatService.GetConversationMessagesAsync(conversationId, lastMessageId, loadOlder);

            // Mark messages as read when user opens conversation
            await _chatService.MarkMessagesAsReadAsync(conversationId);

            return Json(messages);
        }

        /// <summary>
        /// Send message (Fallback if SignalR fails)
        /// </summary>
        [HttpPost] 
        public async Task<IActionResult> SendMessage([FromForm] SendMessageViewModel model)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var message = await _chatService.SendMessageAsync(model);

            return Json(message);
        }

        /// <summary>
        /// Mark messages as read
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int conversationId)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _chatService.MarkMessagesAsReadAsync(conversationId);

            return Json(new { success = result });
        }
    }
}
