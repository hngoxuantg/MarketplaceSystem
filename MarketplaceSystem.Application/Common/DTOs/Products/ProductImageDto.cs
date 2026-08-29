namespace MarketplaceSystem.Application.Common.DTOs.Products
{
    public class ProductImagesDto
    {
        public int TotalImages { get; set; }

        public List<ProductImageDto> Images { get; set; }
    }
    public class ProductImageDto
    {
        public int Id { get; set; }

        public string ImageUrl { get; set; }
        public bool IsMain { get; set; }
    }
}
