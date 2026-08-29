namespace MarketplaceSystem.Application.Common.DTOs.Users
{
    public class AdminUserDto
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string? Avatar { get; set; }

        public UserPostStatsDto PostStats { get; set; }

        public DateTime CreatedAt { get; set; }

        public AdminUserStatus Status { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }
    }
    public class UserPostStatsDto
    {
        public int Pending { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }

        public int Total => Pending + Approved + Rejected;
    }
    public enum AdminUserStatus
    {
        Active = 1,
        Locked = 2
    }
}
