using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Enums.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBusinessRepositories;
using MarketplaceSystem.Infrastructure.Data.Contexts;
using MarketplaceSystem.Infrastructure.Data.Repositories.BaseRepositories;
using Microsoft.EntityFrameworkCore;

namespace MarketplaceSystem.Infrastructure.Data.Repositories.BusinessRepositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(MarketplaceSystemDbContext dbContext) : base(dbContext) { }

        public async Task<Dictionary<DateTime, Dictionary<ProductStatus, int>>> GetProductCountsByDateRangeAsync(
            DateTime fromDate,
            DateTime toDate,
            bool groupByMonth,
            CancellationToken cancellationToken = default)
        {
            if (groupByMonth)
            {
                var monthlyData = await _dbContext.Products
                    .Where(p => p.CreatedAt >= fromDate && p.CreatedAt <= toDate)
                    .GroupBy(p => new { p.CreatedAt.Year, p.CreatedAt.Month, p.Status })
                    .Select(g => new
                    {
                        Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                        Status = g.Key.Status,
                        Count = g.Count()
                    })
                    .ToListAsync(cancellationToken);

                var result = new Dictionary<DateTime, Dictionary<ProductStatus, int>>();
                DateTime currentDate = new DateTime(fromDate.Year, fromDate.Month, 1);
                DateTime endDate = new DateTime(toDate.Year, toDate.Month, 1);

                while (currentDate <= endDate)
                {
                    var statusDict = new Dictionary<ProductStatus, int>();
                    var monthData = monthlyData.Where(x => x.Date == currentDate);

                    foreach (var data in monthData)
                    {
                        statusDict[data.Status] = data.Count;
                    }

                    result[currentDate] = statusDict;
                    currentDate = currentDate.AddMonths(1);
                }

                return result;
            }
            else
            {
                var dailyData = await _dbContext.Products
                    .Where(p => p.CreatedAt >= fromDate.Date && p.CreatedAt <= toDate)
                    .GroupBy(p => new { p.CreatedAt.Date, p.Status })
                    .Select(g => new
                    {
                        Date = g.Key.Date,
                        Status = g.Key.Status,
                        Count = g.Count()
                    })
                    .ToListAsync(cancellationToken);

                var result = new Dictionary<DateTime, Dictionary<ProductStatus, int>>();

                for (DateTime date = fromDate.Date; date <= toDate.Date; date = date.AddDays(1))
                {
                    var statusDict = new Dictionary<ProductStatus, int>();
                    var dayData = dailyData.Where(x => x.Date == date);

                    foreach (var data in dayData)
                    {
                        statusDict[data.Status] = data.Count;
                    }

                    result[date] = statusDict;
                }

                return result;
            }
        }

        public async Task<Dictionary<string, int>> GetStatusDistributionAsync(
            DateTime toDate,
            CancellationToken cancellationToken = default)
        {
            var products = await _dbContext.Products
                .Where(p => p.CreatedAt <= toDate)
                .GroupBy(p => p.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync(cancellationToken);

            return new Dictionary<string, int>
            {
                ["Approved"] = products.Where(p => p.Status == ProductStatus.Active || p.Status == ProductStatus.Sold).Sum(p => p.Count),
                ["Pending"] = products.Where(p => p.Status == ProductStatus.PendingApproval).Sum(p => p.Count),
                ["Rejected"] = products.Where(p => p.Status == ProductStatus.Rejected).Sum(p => p.Count)
            };
        }

        public async Task<List<(int CategoryId, string CategoryName, int ProductCount)>> GetProductsByParentCategoryAsync(
            DateTime toDate,
            CancellationToken cancellationToken = default)
        {
            var result = await _dbContext.Products
                .Where(p => p.CreatedAt <= toDate && p.Category != null)
                .GroupBy(p => p.Category!.ParentCategoryId != null
                    ? p.Category.ParentCategory!.Id
                    : p.Category.Id)
                .Select(g => new
                {
                    CategoryId = g.Key,
                    CategoryName = g.First().Category!.ParentCategoryId != null
                        ? g.First().Category.ParentCategory!.Name
                        : g.First().Category.Name,
                    ProductCount = g.Count()
                })
                .ToListAsync(cancellationToken);

            return result.Select(x => (x.CategoryId, x.CategoryName, x.ProductCount)).ToList();
        }

        public async Task<List<(int CategoryId, string CategoryName, string ParentCategoryName, int ProductCount)>> GetTopCategoriesAsync(
            int count,
            DateTime toDate,
            CancellationToken cancellationToken = default)
        {
            var result = await _dbContext.Products
                .Where(p => p.CreatedAt <= toDate && p.Category != null)
                .GroupBy(p => new
                {
                    CategoryId = p.Category!.Id,
                    CategoryName = p.Category.Name,
                    ParentCategoryName = p.Category.ParentCategory != null ? p.Category.ParentCategory.Name : "Không có"
                })
                .Select(g => new
                {
                    g.Key.CategoryId,
                    g.Key.CategoryName,
                    g.Key.ParentCategoryName,
                    ProductCount = g.Count()
                })
                .OrderByDescending(x => x.ProductCount)
                .Take(count)
                .ToListAsync(cancellationToken);

            return result.Select(x => (x.CategoryId, x.CategoryName, x.ParentCategoryName, x.ProductCount)).ToList();
        }

        public async Task<double> GetAverageViewsInMonthAsync(
            int month,
            int year,
            CancellationToken cancellationToken = default)
        {
            var products = await _dbContext.Products
                .Where(p => p.CreatedAt.Month <= month && p.CreatedAt.Year <= year)
                .Select(p => p.ViewCount)
                .ToListAsync(cancellationToken);

            return products.Any() ? products.Average() : 0;
        }
    }
}
