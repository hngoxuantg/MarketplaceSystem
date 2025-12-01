using MarketplaceSystem.Domain.Entities.Identity_Auth;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IIdentity_AuthRepositories;
using MarketplaceSystem.Infrastructure.Data.Contexts;
using MarketplaceSystem.Infrastructure.Data.Repositories.BaseRepositories;

namespace MarketplaceSystem.Infrastructure.Data.Repositories.Identity_AuthRepositories
{
    public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(MarketplaceSystemDbContext dbContext) : base(dbContext)
        {
        }
    }
}
