namespace MarketplaceSystem.Application.Common.DTOs.Statistics.Users
{
    public class SummaryDto
    {
        public int TotalUsers { get; set; }

        public PeriodStatsDto PeriodStats { get; set; } = null!;

        public int ActiveUsers { get; set; }

        public int ActiveUsersPercentage { get; set; }

        public ActiveUsersPeriodStatsDto ActiveUsersPeriodStats { get; set; } = null!;

        public int NewUsersThisMonth { get; set; }

        public int NewUsersLastMonth { get; set; }

        public double NewUsersChange { get; set; }

        public List<TimelineStatsDto> Timeline { get; set; } = null!;

        public UserStatusDistributionDto StatusDistribution { get; set; } = null!;

        public List<TopActiveUserDto> TopActiveUsers { get; set; } = null!;

        public GenderDistributionDto GenderDistribution { get; set; } = null!;

        public AgeDistributionDto AgeDistribution { get; set; } = null!;

        public LocationDistributionDto LocationDistribution { get; set; } = null!;
    }
    public class PeriodStatsDto
    {
        public int StartPeriodUsers { get; set; }

        public int EndPeriodUsers { get; set; }

        public int NewUsers { get; set; }

        public double GrowthPercentage { get; set; }
    }

    public class ActiveUsersPeriodStatsDto
    {
        public int StartPeriodActiveUsers { get; set; }

        public int EndPeriodActiveUsers { get; set; }

        public int NewActiveUsers { get; set; }

        public double ActiveUsersGrowthPercentage { get; set; }
    }

    public class TimelineStatsDto
    {
        public string Period { get; set; } = null!;

        public int TotalUsers { get; set; }

        public int NewUsers { get; set; }
    }

    public class UserStatusDistributionDto
    {
        public int Active { get; set; }

        public int Locked { get; set; }

        public int PendingEmailConfirmation { get; set; }
    }

    public class TopActiveUserDto
    {
        public int UserId { get; set; }

        public string UserName { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Avatar { get; set; } = null!;

        public int TotalProductViews { get; set; }
    }

    public class GenderDistributionDto
    {
        public int Male { get; set; }

        public int Female { get; set; }

        public int Other { get; set; }
    }

    public class AgeDistributionDto
    {
        public int Under18 { get; set; }

        public int From18To24 { get; set; }

        public int From25To34 { get; set; }

        public int From35To44 { get; set; }

        public int From45To54 { get; set; }

        public int From55To64 { get; set; }

        public int Above65 { get; set; }
    }

    public class LocationDistributionDto
    {
        public List<LocationStatsDto> TopLocations { get; set; } = null!;

        public double OtherPercentage { get; set; }
    }

    public class LocationStatsDto
    {
        public string LocationName { get; set; } = null!;

        public int UserCount { get; set; }

        public double Percentage { get; set; }
    }
}