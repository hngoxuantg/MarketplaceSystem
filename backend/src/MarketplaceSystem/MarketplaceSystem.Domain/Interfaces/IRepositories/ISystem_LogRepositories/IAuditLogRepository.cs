using MarketplaceSystem.Domain.Entities.System_Log;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;

namespace MarketplaceSystem.Domain.Interfaces.IRepositories.ISystem_LogRepositories
{
    public interface IAuditLogRepository : IBaseRepository<AuditLog>
    {
    }
}
