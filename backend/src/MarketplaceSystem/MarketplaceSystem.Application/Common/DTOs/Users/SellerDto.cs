namespace MarketplaceSystem.Application.Common.DTOs.Users
{
    public class SellerDto
    {
        public int UserId { get; set; }
        public string DisplayName { get; set; }
        public string PhoneNumber { get; set; }
        public string AvatarUrl { get; set; }

        public int TotalProducts { get; set; }
        public int TotalSold { get; set; }
    }
}
