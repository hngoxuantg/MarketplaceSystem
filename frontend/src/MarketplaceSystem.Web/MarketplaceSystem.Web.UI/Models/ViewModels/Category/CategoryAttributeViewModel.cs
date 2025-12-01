using System.Text.Json.Serialization;

namespace MarketplaceSystem.Web.UI.Models.ViewModels.Category
{
    public class CategoryAttributeViewModel
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public AttributeType AttributeType { get; set; }

        public bool IsRequired { get; set; }

        public int DisplayOrder { get; set; }

        public string? Placeholder { get; set; }
        public List<AttributeOptionViewModel>? Options { get; set; }
    }
    public class AttributeOptionViewModel
    {
        public int Id { get; set; }

        public string Value { get; set; }

        public string DisplayText { get; set; }

        public bool IsActive { get; set; }

        public int? CategoryAttributeId { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AttributeType
    {
        Text = 1,
        Number = 2,
        Select = 3,
        Boolean = 5,
        Date = 6,
        DateTime = 7,
        Email = 8,
        Phone = 9,
        Url = 10,
        TextArea = 11,
        File = 12,
        Image = 13
    }
}
