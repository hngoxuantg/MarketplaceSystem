using MarketplaceSystem.Domain.Entities.Base;
using MarketplaceSystem.Domain.Entities.Identity_Auth;

namespace MarketplaceSystem.Domain.Entities.Business
{
    public class Conversation : BaseEntity
    {
        public int UserAId { get; set; }
        public User? User { get; set; }

        public int UserBId { get; set; }
        public User? UserB { get; set; }

        public int? LastMessageId { get; set; }
        public Message? Message { get; set; }

        private readonly List<Message> _messages = new List<Message>();
        public IReadOnlyCollection<Message>? Messages => _messages.AsReadOnly();

        public Conversation() { }

        public Conversation(
            int userAId,
            int userBId,
            int lastMessageId)
        {
            UserAId = userAId;
            UserBId = userBId;
            LastMessageId = lastMessageId;
        }

        public void AddMessage(Message message)
        {
            _messages.Add(message);
        }

        public void SetLastMessage(int messageId)
        {
            LastMessageId = messageId;
        }

        public void MarkAllMessagesAsRead()
        {
            foreach (var message in _messages)
            {
                message.MarkAsRead();
            }
        }
    }
}
