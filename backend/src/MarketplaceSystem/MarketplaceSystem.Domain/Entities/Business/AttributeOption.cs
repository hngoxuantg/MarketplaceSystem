using MarketplaceSystem.Domain.Entities.Base;

namespace MarketplaceSystem.Domain.Entities.Business
{
    public class AttributeOption : SoftDeleteEntity
    {
        public string Value { get; set; } //Giá trị: VD: Honda

        public string DisplayText { get; set; } //Tên hiển thị Vd: Honda(Nhật Bản)

        public bool IsActive { get; set; } = true;

        public int? CategoryAttributeId { get; set; }

        public CategoryAttribute? CategoryAttribute { get; set; }
        public AttributeOption()
        {
        }
        public AttributeOption(string value, string displayText)
        {
            Value = value;
            DisplayText = displayText;
        }
    }
}
