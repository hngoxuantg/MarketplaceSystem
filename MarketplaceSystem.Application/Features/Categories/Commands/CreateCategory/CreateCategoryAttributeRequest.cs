using MarketplaceSystem.Domain.Enums.Business;

namespace MarketplaceSystem.Application.Features.Categories.Commands.CreateCategory
{
    public record CreateCategoryAttributeRequest
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public AttributeType AttributeType { get; set; }

        public bool IsRequired { get; set; }

        public int DisplayOrder { get; set; }

        public string? Placeholder { get; set; }

        public List<CreateAttributeOptionRequest>? AttributeOptions { get; set; }
    }
}
