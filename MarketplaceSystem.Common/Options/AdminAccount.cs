namespace MarketplaceSystem.Common.Options
{
    public class AdminAccount
    {
        public const string SectionName = "AdminAccount";

        public Account? Account { get; set; }
        public Profile? Profile { get; set; }
    }
    public class Account
    {
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
    public class Profile
    {
        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
    }
}
