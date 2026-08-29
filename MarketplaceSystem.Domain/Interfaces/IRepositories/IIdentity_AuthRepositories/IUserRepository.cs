using MarketplaceSystem.Domain.Entities.Identity_Auth;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;

namespace MarketplaceSystem.Domain.Interfaces.IRepositories.IIdentity_AuthRepositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<Dictionary<string, int>> GetStatusDistributionAsync(
            DateTime toDate,
            CancellationToken cancellationToken = default);

        Task<List<(int UserId, string UserName, string FullName, string Avatar, int TotalViews)>> GetTopActiveUsersAsync(
            int count,
            CancellationToken cancellationToken = default);
    }
}
