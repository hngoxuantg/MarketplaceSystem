using MarketplaceSystem.Domain.Entities.Base;

namespace MarketplaceSystem.Domain.Entities.Business
{
    public class ProductAttributeValue : SoftDeleteEntity
    {
        public string? TextValue { get; set; }
        public decimal? NumberValue { get; set; }
        public bool? BooleanValue { get; set; }
        public DateTime? DateValue { get; set; }
        public string? SelectValues { get; set; }

        public int? ProductId { get; set; }
        public virtual Product? Product { get; set; }

        public int? CategoryAttributeId { get; set; }
        public CategoryAttribute? CategoryAttribute { get; set; }

        public ProductAttributeValue()
        {
        }

        public ProductAttributeValue(
            int categoryAttributeId,
            string? textValue,
            decimal? numberValue,
            bool? booleanValue,
            DateTime? dateValue,
            string? selectValues)
        {
            CategoryAttributeId = categoryAttributeId;
            TextValue = textValue;
            NumberValue = numberValue;
            BooleanValue = booleanValue;
            DateValue = dateValue;
            SelectValues = selectValues;
        }
    }
}
