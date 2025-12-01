using MarketplaceSystem.Web.UI.Admin.Models.ViewModel.Shared;
using System.Text.Json.Serialization;
using MarketplaceSystem.Web.UI.Admin.Converters;

namespace MarketplaceSystem.Web.UI.Admin.Models.ViewModel.Categories
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public string? Icon { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }

        public int? ParentCategoryId { get; set; }

        public string? ParentCategoryName { get; set; }

        public List<CategoryAttributeViewModel>? Attributes { get; set; }
        public List<CategoryViewModel>? SubCategories { get; set; }

        public bool? IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int SubCategoriesCount => SubCategories?.Count ?? 0;

        public bool IsParentCategory => ParentCategoryId == null;

        public bool IsChildCategory => ParentCategoryId != null;

        public bool HasSubCategories => SubCategoriesCount > 0;
    }
    public class CategoryAttributeViewModel
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        [JsonConverter(typeof(AttributeTypeConverter))]
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
}
