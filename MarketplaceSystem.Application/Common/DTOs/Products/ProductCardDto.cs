using MarketplaceSystem.Application.Common.DTOs.Users;

namespace MarketplaceSystem.Application.Common.DTOs.Products
{
    public class ProductCardDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }

        public string Condition { get; set; }
        public string? Status { get; set; }

        public string Location { get; set; }
        public string CategoryName { get; set; }
        public DateTime CreateAt { get; set; }

        public SellerDto? Seller { get; set; }
    }
}
