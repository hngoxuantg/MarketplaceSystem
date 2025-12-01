namespace MarketplaceSystem.Web.UI.Admin.Models.ViewModel.Products
{
    public class ProductListViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public string? Condition { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
        public SellerInfo? Seller { get; set; }
        public string? Flag { get; set; }
        public string? WarningDetail { get; set; }
    }

    public class ProductDetailViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public string? Condition { get; set; }
        public string ProductStatus { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public SellerInfo? Seller { get; set; }
        public List<string>? ImageUrls { get; set; }
        public List<ProductAttributeValue>? AttributeValues { get; set; }
        public string? RejectionReason { get; set; }
        public string? Flag { get; set; }
        public string? WarningDetail { get; set; }
    }

    public class ProductAttributeValue
    {
        public string AttributeName { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string DisplayValue { get; set; } = string.Empty;
    }

    public class SellerInfo
    {
        public int UserId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? AvatarUrl { get; set; }
        public int TotalProducts { get; set; }
        public int TotalSold { get; set; }
    }

    public class ProductFilterViewModel
    {
        public int? Status { get; set; }
        public int? CategoryId { get; set; }
        public int? SortBy { get; set; }
        public string? Search { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }
}
