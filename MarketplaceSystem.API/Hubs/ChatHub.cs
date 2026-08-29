using MarketplaceSystem.Application.Features.Chat.Commands.MarkMessagesAsRead;
using MarketplaceSystem.Application.Features.Chat.Commands.SendMessage;
using MarketplaceSystem.Application.Features.Chat.Queries.GetConversationMessages;
using MarketplaceSystem.Application.Features.Chat.Queries.GetConversations;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace MarketplaceSystem.API.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatHub : Hub
    {
        private readonly ISender _sender;
        public ChatHub(ISender sender)
        {
            _sender = sender;
        }

        public override async Task OnConnectedAsync()
        {
            string? userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                Context.Abort();
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, userId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendPrivateMessage(SendMessageRequest request)
        {
            string? userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _sender.Send(new SendMessageCommand(int.Parse(userId), request));

            await Clients.Group(request.ReceiverId.ToString())
                .SendAsync("ReceivePrivateMessage", result);
        }

        public async Task GetConversationMessages(int conversationId, int? lastMessageId, bool loadOlder)
        {
            var messages = await _sender.Send(new GetConversationMessagesQuery(conversationId, lastMessageId, loadOlder));

            await Clients.Caller.SendAsync("ReceiveMessages", messages);
        }

        public async Task MarkMessagesAsRead(int conversationId)
        {
            string? userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                Context.Abort();
                return;
            }

            var receiverId = await _sender.Send(new MarkMessagesAsReadCommand(int.Parse(userId), conversationId));

            await Clients.Group(receiverId.ToString())
                .SendAsync("MessagesRead", receiverId, int.Parse(userId));
        }

        public async Task GetConversations(GetConversationsRequest request)
        {
            string? userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                Context.Abort();
                return;
            }

            var result = await _sender.Send(new GetConversationsQuery(int.Parse(userId), request));

            await Clients.Caller.SendAsync("ReceiveConversations", result);
        }

    }
}
