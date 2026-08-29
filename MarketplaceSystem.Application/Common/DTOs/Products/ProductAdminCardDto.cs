using MarketplaceSystem.Domain.Enums.Business;

namespace MarketplaceSystem.Application.Common.DTOs.Products
{
    public class ProductAdminCardDto : ProductCardDto
    {
        public string? Description { get; set; }

        public ContentWarningFlag Flag { get; set; }

        public string? WarningDetail { get; set; }
    }
}
