using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBusinessRepositories;
using MarketplaceSystem.Infrastructure.Data.Contexts;
using MarketplaceSystem.Infrastructure.Data.Repositories.BaseRepositories;

namespace MarketplaceSystem.Infrastructure.Data.Repositories.BusinessRepositories
{
    public class AttributeOptionRepository : BaseRepository<AttributeOption>, IAttributeOptionRepository
    {
        public AttributeOptionRepository(MarketplaceSystemDbContext dbContext) : base(dbContext) { }
    }
}
