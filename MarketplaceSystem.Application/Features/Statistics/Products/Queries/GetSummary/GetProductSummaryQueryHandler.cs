using MarketplaceSystem.Application.Common.DTOs.Statistics.Products;
using MarketplaceSystem.Domain.Enums.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;

namespace MarketplaceSystem.Application.Features.Statistics.Products.Queries.GetSummary
{
    public class GetProductSummaryQueryHandler : IRequestHandler<GetProductSummaryQuery, ProductSummaryDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProductSummaryQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductSummaryDto> Handle(GetProductSummaryQuery query, CancellationToken cancellationToken = default)
        {
            return await GetSummaryAsync(query.Request, cancellationToken);
        }

        private async Task<ProductSummaryDto> GetSummaryAsync(
            GetProductSummaryRequest request,
            CancellationToken cancellationToken = default)
        {
            DateTime systemStartDate = await GetSystemStartDateAsync(cancellationToken);

            if (request.From < systemStartDate)
            {
                request.From = systemStartDate;
            }
            if (request.To > DateTime.UtcNow)
            {
                request.To = DateTime.UtcNow;
            }

            ProductSummaryDto summaryDto = new ProductSummaryDto();

            summaryDto.TotalProducts = await GetTotalProductsAsync(request, cancellationToken);
            summaryDto.PeriodStats = await GetPeriodStatsAsync(request, cancellationToken);
            summaryDto.Timeline = await GetTimelineStatsAsync(request, cancellationToken);
            summaryDto.StatusDistribution = await GetStatusDistributionAsync(request, cancellationToken);
            summaryDto.ParentCategoryStats = await GetParentCategoryStatsAsync(request, cancellationToken);
            summaryDto.TopCategories = await GetTopCategoriesAsync(request, cancellationToken);

            DateTime now = DateTime.UtcNow;
            summaryDto.AverageViewsThisMonth = await _unitOfWork.ProductRepository.GetAverageViewsInMonthAsync(now.Month, now.Year, cancellationToken);
            summaryDto.AverageViewsLastMonth = await _unitOfWork.ProductRepository.GetAverageViewsInMonthAsync(now.AddMonths(-1).Month, now.AddMonths(-1).Year, cancellationToken);
            summaryDto.ViewsChange = CalculatePercentageChange(summaryDto.AverageViewsThisMonth, summaryDto.AverageViewsLastMonth);

            return summaryDto;
        }

        private async Task<DateTime> GetSystemStartDateAsync(CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.ProductRepository.GetOneUntrackedAsync(
                orderBy: q => q.OrderBy(p => p.CreatedAt),
                selector: p => p.CreatedAt,
                cancellation: cancellationToken
            );
        }

        private async Task<int> GetTotalProductsAsync(GetProductSummaryRequest request, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.ProductRepository.GetCountAsync(
                filter: p => p.CreatedAt <= request.To,
                cancellation: cancellationToken
            );
        }

        private async Task<ProductPeriodStatsDto> GetPeriodStatsAsync(GetProductSummaryRequest request, CancellationToken cancellationToken = default)
        {
            int startPeriodProducts = await _unitOfWork.ProductRepository.GetCountAsync(
                filter: p => p.CreatedAt < request.From,
                cancellation: cancellationToken
            );

            int endPeriodProducts = await _unitOfWork.ProductRepository.GetCountAsync(
                filter: p => p.CreatedAt <= request.To,
                cancellation: cancellationToken
            );

            int newProducts = await _unitOfWork.ProductRepository.GetCountAsync(
                filter: p => p.CreatedAt >= request.From && p.CreatedAt <= request.To,
                cancellation: cancellationToken
            );

            double growthPercentage = CalculateGrowthPercentage(startPeriodProducts, endPeriodProducts);

            return new ProductPeriodStatsDto
            {
                StartPeriodProducts = startPeriodProducts,
                EndPeriodProducts = endPeriodProducts,
                NewProducts = newProducts,
                GrowthPercentage = growthPercentage
            };
        }

        private double CalculateGrowthPercentage(int startValue, int endValue)
        {
            if (startValue == 0)
            {
                return endValue == 0 ? 0 : 100;
            }

            return ((double)(endValue - startValue) / startValue) * 100;
        }

        private async Task<List<ProductTimelineStatsDto>> GetTimelineStatsAsync(
            GetProductSummaryRequest request,
            CancellationToken cancellationToken = default)
        {
            TimeSpan duration = request.To - request.From;
            bool isMonthly = duration.TotalDays > 60;

            var productCountsByDate = await _unitOfWork.ProductRepository.GetProductCountsByDateRangeAsync(
                request.From,
                request.To,
                isMonthly,
                cancellationToken
            );

            List<ProductTimelineStatsDto> timeline = new List<ProductTimelineStatsDto>();

            int cumulativeTotal = await _unitOfWork.ProductRepository.GetCountAsync(
                filter: p => p.CreatedAt < request.From,
                cancellation: cancellationToken
            );

            foreach (var kvp in productCountsByDate.OrderBy(x => x.Key))
            {
                string periodFormat = isMonthly ? kvp.Key.ToString("yyyy-MM") : kvp.Key.ToString("yyyy-MM-dd");

                int newProducts = kvp.Value.Values.Sum();
                cumulativeTotal += newProducts;

                timeline.Add(new ProductTimelineStatsDto
                {
                    Period = periodFormat,
                    TotalProducts = cumulativeTotal,
                    NewProducts = newProducts,
                    ApprovedProducts = kvp.Value.GetValueOrDefault(ProductStatus.Active, 0) + kvp.Value.GetValueOrDefault(ProductStatus.Sold, 0),
                    PendingProducts = kvp.Value.GetValueOrDefault(ProductStatus.PendingApproval, 0),
                    RejectedProducts = kvp.Value.GetValueOrDefault(ProductStatus.Rejected, 0)
                });
            }

            return timeline;
        }

        private async Task<ProductStatusDistributionDto> GetStatusDistributionAsync(
            GetProductSummaryRequest request,
            CancellationToken cancellationToken = default)
        {
            var statusData = await _unitOfWork.ProductRepository.GetStatusDistributionAsync(
                request.To,
                cancellationToken
            );

            return new ProductStatusDistributionDto
            {
                Approved = statusData.GetValueOrDefault("Approved", 0),
                Pending = statusData.GetValueOrDefault("Pending", 0),
                Rejected = statusData.GetValueOrDefault("Rejected", 0)
            };
        }

        private async Task<List<ParentCategoryStatsDto>> GetParentCategoryStatsAsync(
            GetProductSummaryRequest request,
            CancellationToken cancellationToken = default)
        {
            var categoryData = await _unitOfWork.ProductRepository.GetProductsByParentCategoryAsync(
                request.To,
                cancellationToken
            );

            int totalProducts = categoryData.Sum(x => x.ProductCount);
            if (totalProducts == 0)
            {
                return new List<ParentCategoryStatsDto>();
            }

            return categoryData.Select(x => new ParentCategoryStatsDto
            {
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName,
                ProductCount = x.ProductCount,
                Percentage = (double)x.ProductCount / totalProducts * 100
            }).ToList();
        }

        private async Task<List<TopCategoryStatsDto>> GetTopCategoriesAsync(
            GetProductSummaryRequest request,
            CancellationToken cancellationToken = default)
        {
            var topCategories = await _unitOfWork.ProductRepository.GetTopCategoriesAsync(
                10,
                request.To,
                cancellationToken
            );

            return topCategories.Select(x => new TopCategoryStatsDto
            {
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName,
                ParentCategoryName = x.ParentCategoryName,
                ProductCount = x.ProductCount
            }).ToList();
        }

        private double CalculatePercentageChange(double currentValue, double previousValue)
        {
            if (previousValue == 0)
            {
                return currentValue == 0 ? 0 : 100;
            }

            return ((currentValue - previousValue) / previousValue) * 100;
        }
    }
}
