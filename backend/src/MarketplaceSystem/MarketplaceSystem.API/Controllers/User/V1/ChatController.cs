using MediatR;

namespace MarketplaceSystem.API.Controllers.User.V1
{
    public class ChatController : BaseController
    {
        private readonly ISender _sender;
        public ChatController(ISender sender)
        {
            _sender = sender;
        }

    }
}
