namespace MarketplaceSystem.Web.UI.Admin.Models.ViewModel.Statistics
{
    public class PostStatisticsViewModel
    {
        public int TotalProducts { get; set; }
        public PostPeriodStatistic PeriodStats { get; set; } = new();
        public List<PostTimelinePoint> Timeline { get; set; } = new();
        public PostStatusDistribution StatusDistribution { get; set; } = new();
        public List<ParentCategoryStatistic> ParentCategoryStats { get; set; } = new();
        public List<TopCategoryStatistic> TopCategories { get; set; } = new();
        public decimal AverageViewsThisMonth { get; set; }
        public decimal AverageViewsLastMonth { get; set; }
        public decimal ViewsChange { get; set; }
    }

    public class PostPeriodStatistic
    {
        public int StartPeriodProducts { get; set; }
        public int EndPeriodProducts { get; set; }
        public int NewProducts { get; set; }
        public decimal GrowthPercentage { get; set; }
    }

    public class PostTimelinePoint
    {
        public DateTime Period { get; set; }
        public int TotalProducts { get; set; }
        public int NewProducts { get; set; }
        public int ApprovedProducts { get; set; }
        public int PendingProducts { get; set; }
        public int RejectedProducts { get; set; }
    }

    public class PostStatusDistribution
    {
        public int Approved { get; set; }
        public int Pending { get; set; }
        public int Rejected { get; set; }
    }

    public class ParentCategoryStatistic
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int ProductCount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class TopCategoryStatistic
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string ParentCategoryName { get; set; } = string.Empty;
        public int ProductCount { get; set; }
    }
}
