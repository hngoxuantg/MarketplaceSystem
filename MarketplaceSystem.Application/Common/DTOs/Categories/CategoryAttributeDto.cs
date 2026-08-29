using MarketplaceSystem.Domain.Enums.Business;

namespace MarketplaceSystem.Application.Common.DTOs.Categories
{
    public class CategoryAttributeDto
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public AttributeType AttributeType { get; set; }

        public bool IsRequired { get; set; }

        public int DisplayOrder { get; set; }

        public string? Placeholder { get; set; }
        public List<AttributeOptionDto>? Options { get; set; }
    }
}
