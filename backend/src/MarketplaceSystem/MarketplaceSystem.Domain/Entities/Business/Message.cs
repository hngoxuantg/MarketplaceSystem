using MarketplaceSystem.Domain.Entities.Base;
using MarketplaceSystem.Domain.Entities.Identity_Auth;
using MarketplaceSystem.Domain.Enums.Business;

namespace MarketplaceSystem.Domain.Entities.Business
{
    public class Message : BaseEntity
    {
        public int SenderId { get; set; }
        public User? Sender { get; set; }

        public int ReceiverId { get; set; }
        public User? Receiver { get; set; }

        public int ConversationId { get; set; }
        public Conversation? Conversation { get; set; }

        public string Content { get; private set; }

        public MessageType MessageType { get; private set; }

        public bool IsRead { get; private set; }

        public Message() { }

        public Message(
            int senderId,
            int receiverId)
        {
            SenderId = senderId;
            ReceiverId = receiverId;
            IsRead = false;
        }

        public void MarkAsRead()
        {
            IsRead = true;
        }

        public void SetType(MessageType type)
        {
            MessageType = type;
        }

        public void SetContent(string content)
        {
            Content = content;
        }
    }
}
