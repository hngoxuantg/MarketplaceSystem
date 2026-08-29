using MarketplaceSystem.Domain.Entities.Base;

namespace MarketplaceSystem.Domain.Entities.Business
{
    public class ProductImage : SoftDeleteEntity
    {
        public string ImageUrl { get; set; }
        public string? AltText { get; set; }
        public bool IsMain { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
