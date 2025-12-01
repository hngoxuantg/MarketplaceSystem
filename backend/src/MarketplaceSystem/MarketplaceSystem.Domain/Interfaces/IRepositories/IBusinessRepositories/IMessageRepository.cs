using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using System.Linq.Expressions;

namespace MarketplaceSystem.Domain.Interfaces.IRepositories.IBusinessRepositories
{
    public interface IMessageRepository : IBaseRepository<Message>
    {
        Task<IEnumerable<TResult>> GetMessagesAsync<TResult>(
            int conversationId,
            Expression<Func<Message, bool>> filter,
            int take,
            Expression<Func<Message, TResult>>? selector = null,
            CancellationToken cancellation = default);
    }
}
