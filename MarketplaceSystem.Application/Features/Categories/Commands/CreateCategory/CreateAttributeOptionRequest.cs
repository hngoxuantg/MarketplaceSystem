namespace MarketplaceSystem.Application.Features.Categories.Commands.CreateCategory
{
    public record CreateAttributeOptionRequest
    {
        public string Value { get; set; }
        public string DisplayText { get; set; }
    }
}
