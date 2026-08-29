namespace MarketplaceSystem.Application.Features.Products.Commands.CreateProduct
{
    public record CreateProductAttributeValueRequest
    {
        public int CategoryAttributeId { get; set; }

        public string? TextValue { get; set; }

        public decimal? NumberValue { get; set; }

        public bool? BooleanValue { get; set; }

        public DateTime? DateValue { get; set; }

        public string? SelectValues { get; set; }

        public bool HasValue()
        {
            return TextValue is not null
                || NumberValue is not null
                || BooleanValue is not null
                || DateValue is not null
                || SelectValues is not null;
        }
    }
}
