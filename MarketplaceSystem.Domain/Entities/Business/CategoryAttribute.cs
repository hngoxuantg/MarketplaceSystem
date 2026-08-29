using MarketplaceSystem.Domain.Entities.Base;
using MarketplaceSystem.Domain.Enums.Business;

namespace MarketplaceSystem.Domain.Entities.Business
{
    public class CategoryAttribute : SoftDeleteEntity
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public AttributeType AttributeType { get; set; }
        public bool IsRequired { get; set; }
        public int DisplayOrder { get; set; }
        public string? Placeholder { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }


        private readonly List<AttributeOption> _attributeOptions = new List<AttributeOption>();
        public IReadOnlyCollection<AttributeOption>? AttributeOptions => _attributeOptions.AsReadOnly();

        public CategoryAttribute()
        {
        }
        public CategoryAttribute(string name, string displayName, AttributeType attributeType, bool isRequired, string? placeholder)
        {
            Name = name;
            DisplayName = displayName;
            AttributeType = attributeType;
            IsRequired = isRequired;
            Placeholder = placeholder;
        }
        public void AddRangeAttributeOption(IEnumerable<AttributeOption> option)
        {
            _attributeOptions.AddRange(option);
        }
    }
}