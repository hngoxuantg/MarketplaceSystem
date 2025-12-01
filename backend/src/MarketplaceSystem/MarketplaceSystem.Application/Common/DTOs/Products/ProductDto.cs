using MarketplaceSystem.Application.Common.DTOs.Users;

namespace MarketplaceSystem.Application.Common.DTOs.Products
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Location { get; set; }
        public decimal Price { get; set; }
        public string ProductStatus { get; set; }
        public string Condition { get; set; }
        public int Quantity { get; set; }

        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }

        public ProductImagesDto Images { get; set; }

        public List<ProductAttributeValueDto> AttributeValues { get; set; }

        public SellerDto Seller { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
