using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBusinessRepositories;
using MarketplaceSystem.Infrastructure.Data.Contexts;
using MarketplaceSystem.Infrastructure.Data.Repositories.BaseRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MarketplaceSystem.Infrastructure.Data.Repositories.BusinessRepositories
{
    public class MessageRepository : BaseRepository<Message>, IMessageRepository
    {
        public MessageRepository(MarketplaceSystemDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<TResult>> GetMessagesAsync<TResult>(
            int conversationId,
            Expression<Func<Message, bool>> filter,
            int take,
            Expression<Func<Message, TResult>>? selector = null,
            CancellationToken cancellation = default)
        {
            IQueryable<Message> query = _dbContext.Messages
                .AsNoTracking()
                .Where(m => m.ConversationId == conversationId)
                .Where(filter)
                .OrderByDescending(m => m.Id)
                .Take(take);

            if (selector != null)
                return await query.Select(selector).ToListAsync(cancellation);
            else
                return await query.Cast<TResult>().ToListAsync(cancellation);
        }
    }
}
