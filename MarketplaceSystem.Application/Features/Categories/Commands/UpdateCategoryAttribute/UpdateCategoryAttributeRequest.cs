namespace MarketplaceSystem.Application.Features.Categories.Commands.UpdateCategoryAttribute
{
    public class UpdateCategoryAttributeRequest
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public bool IsRequired { get; set; }

        public int DisplayOrder { get; set; }

        public string? Placeholder { get; set; }
    }
}
