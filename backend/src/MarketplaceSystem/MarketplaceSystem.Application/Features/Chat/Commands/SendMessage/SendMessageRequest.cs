using Microsoft.AspNetCore.Http;

namespace MarketplaceSystem.Application.Features.Chat.Commands.SendMessage
{
    public class SendMessageRequest
    {
        public int ReceiverId { get; set; }

        public string? Content { get; set; }

        public IFormFile? Image { get; set; }
    }
}
