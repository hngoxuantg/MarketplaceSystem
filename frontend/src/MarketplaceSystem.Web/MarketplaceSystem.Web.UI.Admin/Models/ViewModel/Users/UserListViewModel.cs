namespace MarketplaceSystem.Web.UI.Admin.Models.ViewModel.Users;

public class UserListViewModel
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Avatar { get; set; }
    public PostStatsViewModel PostStats { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsEmailConfirmed { get; set; }
    public DateTime? LockoutEnd { get; set; }
}

public class PostStatsViewModel
{
    public int Pending { get; set; }
    public int Approved { get; set; }
    public int Rejected { get; set; }
    public int Total { get; set; }
}
