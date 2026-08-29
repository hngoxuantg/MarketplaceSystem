using MarketplaceSystem.Domain.Entities.System_Log;
using MarketplaceSystem.Domain.Interfaces.IRepositories.ISystem_LogRepositories;
using MarketplaceSystem.Infrastructure.Data.Contexts;
using MarketplaceSystem.Infrastructure.Data.Repositories.BaseRepositories;

namespace MarketplaceSystem.Infrastructure.Data.Repositories.System_LogRepositories
{
    public class AuditLogRepository : BaseRepository<AuditLog>, IAuditLogRepository
    {
        public AuditLogRepository(MarketplaceSystemDbContext dbContext) : base(dbContext) { }
    }
}
