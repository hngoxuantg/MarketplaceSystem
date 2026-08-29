using MarketplaceSystem.Common.Enums;
using MarketplaceSystem.Domain.Enums.Business;

namespace MarketplaceSystem.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductRequest
    {
        public string Title { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public ProductCondition Condition { get; set; }

        public int CategoryId { get; set; }

        public VietnamProvince Location { get; set; }

        public List<CreateProductAttributeValueRequest> AttributeValues { get; set; }
    }
}
