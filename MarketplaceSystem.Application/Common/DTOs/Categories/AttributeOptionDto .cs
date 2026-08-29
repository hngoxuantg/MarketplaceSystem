namespace MarketplaceSystem.Application.Common.DTOs.Categories
{
    public class AttributeOptionDto
    {
        public int Id { get; set; }

        public string Value { get; set; }

        public string DisplayText { get; set; }

        public bool IsActive { get; set; }

        public int? CategoryAttributeId { get; set; }
    }
}
