namespace MarketplaceSystem.Web.UI.Models.ViewModels.Product
{
    public class ProductImagesViewModel
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
