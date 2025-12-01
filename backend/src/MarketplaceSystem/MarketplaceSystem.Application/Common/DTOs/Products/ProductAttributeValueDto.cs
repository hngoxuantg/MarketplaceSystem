using MarketplaceSystem.Domain.Enums.Business;

namespace MarketplaceSystem.Application.Common.DTOs.Products
{
    public class ProductAttributeValueDto
    {
        public int Id { get; set; }
        public AttributeType AttributeType { get; set; }
        public string AttributeName { get; set; }

        public string? TextValue { get; set; }
        public decimal? NumberValue { get; set; }
        public bool? BooleanValue { get; set; }
        public DateTime? DateValue { get; set; }
        public string? SelectsValue { get; set; }
    }
}
