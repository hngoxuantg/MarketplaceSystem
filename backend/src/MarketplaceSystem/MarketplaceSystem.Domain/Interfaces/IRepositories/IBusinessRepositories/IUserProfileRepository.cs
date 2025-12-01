using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;

namespace MarketplaceSystem.Domain.Interfaces.IRepositories.IBusinessRepositories
{
    public interface IUserProfileRepository : IBaseRepository<UserProfile>
    {
        Task<Dictionary<DateTime, int>> GetUserCountsByDateRangeAsync(
            DateTime fromDate,
            DateTime toDate,
            bool groupByMonth,
            CancellationToken cancellationToken = default);

        Task<Dictionary<TEnum, int>> GetDistributionByEnumAsync<TEnum>(
            DateTime toDate,
            Func<UserProfile, TEnum> selector,
            CancellationToken cancellationToken = default) where TEnum : struct, Enum;

        Task<Dictionary<int, int>> GetAgeDistributionAsync(
            DateTime toDate,
            CancellationToken cancellationToken = default);
    }
}
