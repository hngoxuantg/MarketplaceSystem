namespace MarketplaceSystem.Application.Common.DTOs.Statistics.Products
{
    public class ProductSummaryDto
    {
        public int TotalProducts { get; set; }

        public ProductPeriodStatsDto PeriodStats { get; set; } = null!;

        public List<ProductTimelineStatsDto> Timeline { get; set; } = null!;

        public ProductStatusDistributionDto StatusDistribution { get; set; } = null!;

        public List<ParentCategoryStatsDto> ParentCategoryStats { get; set; } = null!;

        public List<TopCategoryStatsDto> TopCategories { get; set; } = null!;

        public double AverageViewsThisMonth { get; set; }

        public double AverageViewsLastMonth { get; set; }

        public double ViewsChange { get; set; }
    }

    public class ProductPeriodStatsDto
    {
        public int StartPeriodProducts { get; set; }

        public int EndPeriodProducts { get; set; }

        public int NewProducts { get; set; }

        public double GrowthPercentage { get; set; }
    }

    public class ProductTimelineStatsDto
    {
        public string Period { get; set; } = null!;

        public int TotalProducts { get; set; }

        public int NewProducts { get; set; }

        public int ApprovedProducts { get; set; }

        public int PendingProducts { get; set; }

        public int RejectedProducts { get; set; }
    }

    public class ProductStatusDistributionDto
    {
        public int Approved { get; set; }

        public int Pending { get; set; }

        public int Rejected { get; set; }
    }

    public class ParentCategoryStatsDto
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = null!;

        public int ProductCount { get; set; }

        public double Percentage { get; set; }
    }

    public class TopCategoryStatsDto
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = null!;

        public string ParentCategoryName { get; set; } = null!;

        public int ProductCount { get; set; }
    }
}
