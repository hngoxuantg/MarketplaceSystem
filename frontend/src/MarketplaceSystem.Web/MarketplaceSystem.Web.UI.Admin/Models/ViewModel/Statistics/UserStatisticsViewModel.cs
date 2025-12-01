using System.Text.Json.Serialization;

namespace MarketplaceSystem.Web.UI.Admin.Models.ViewModel.Statistics
{
    public class UserStatisticsViewModel
    {
        public int TotalUsers { get; set; }
        public PeriodStatistic PeriodStats { get; set; } = new();
        public int ActiveUsers { get; set; }
        public decimal ActiveUsersPercentage { get; set; }
        public ActivePeriodStatistic ActiveUsersPeriodStats { get; set; } = new();
        public int ActiveUsersGrowth => ActiveUsersPeriodStats.NewActiveUsers;
        public int NewUsersThisMonth { get; set; }
        public int NewUsersLastMonth { get; set; }
        public decimal NewUsersChange { get; set; }
        public List<UserTimelinePoint> Timeline { get; set; } = new();
        public UserStatusDistribution StatusDistribution { get; set; } = new();
        public List<TopActiveUser> TopActiveUsers { get; set; } = new();
        public GenderDistribution GenderDistribution { get; set; } = new();
        public AgeDistribution AgeDistribution { get; set; } = new();
        public LocationDistribution LocationDistribution { get; set; } = new();
    }

    public class PeriodStatistic
    {
        public int StartPeriodUsers { get; set; }
        public int EndPeriodUsers { get; set; }
        public int NewUsers { get; set; }
        public decimal GrowthPercentage { get; set; }
    }

    public class ActivePeriodStatistic
    {
        public int StartPeriodActiveUsers { get; set; }
        public int EndPeriodActiveUsers { get; set; }
        public int NewActiveUsers { get; set; }
        public decimal ActiveUsersGrowthPercentage { get; set; }
    }

    public class UserTimelinePoint
    {
        public DateTime Period { get; set; }
        public int TotalUsers { get; set; }
        public int NewUsers { get; set; }
    }

    public class UserStatusDistribution
    {
        public int Active { get; set; }
        public int Locked { get; set; }

        [JsonPropertyName("pendingEmailConfirmation")]
        public int PendingEmailConfirmation { get; set; }
    }

    public class TopActiveUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public int TotalProductViews { get; set; }
    }

    public class GenderDistribution
    {
        public int Male { get; set; }
        public int Female { get; set; }
        public int Other { get; set; }
    }

    public class AgeDistribution
    {
        public int Under18 { get; set; }
        public int From18To24 { get; set; }
        public int From25To34 { get; set; }
        public int From35To44 { get; set; }
        public int From45To54 { get; set; }
        public int From55To64 { get; set; }
        public int Above65 { get; set; }
    }

    public class LocationDistribution
    {
        public List<LocationBreakdown> TopLocations { get; set; } = new();
        public decimal OtherPercentage { get; set; }
    }

    public class LocationBreakdown
    {
        public string LocationName { get; set; } = string.Empty;
        public int UserCount { get; set; }
        public decimal Percentage { get; set; }
    }
}
