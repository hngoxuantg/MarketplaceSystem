using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Enums.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;

namespace MarketplaceSystem.Domain.Interfaces.IRepositories.IBusinessRepositories
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<Dictionary<DateTime, Dictionary<ProductStatus, int>>> GetProductCountsByDateRangeAsync(
            DateTime fromDate,
            DateTime toDate,
            bool groupByMonth,
            CancellationToken cancellationToken = default);

        Task<Dictionary<string, int>> GetStatusDistributionAsync(
            DateTime toDate,
            CancellationToken cancellationToken = default);

        Task<List<(int CategoryId, string CategoryName, int ProductCount)>> GetProductsByParentCategoryAsync(
            DateTime toDate,
            CancellationToken cancellationToken = default);

        Task<List<(int CategoryId, string CategoryName, string ParentCategoryName, int ProductCount)>> GetTopCategoriesAsync(
            int count,
            DateTime toDate,
            CancellationToken cancellationToken = default);

        Task<double> GetAverageViewsInMonthAsync(
            int month,
            int year,
            CancellationToken cancellationToken = default);
    }
}
