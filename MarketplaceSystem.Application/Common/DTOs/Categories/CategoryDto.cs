namespace MarketplaceSystem.Application.Common.DTOs.Categories
{
    public class CategoryDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public string? Icon { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }

        public int? ParentCategoryId { get; set; }

        public string? ParentCategoryName { get; set; }

        public int? ProductsCount { get; set; }

        public List<CategoryAttributeDto>? Attributes { get; set; }

        public List<CategoryDto>? SubCategories { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int AttributesCount => Attributes?.Count ?? 0;

        public int SubCategoriesCount => SubCategories?.Count ?? 0;

        public bool IsParentCategory => ParentCategoryId == null;

        public bool IsChildCategory => ParentCategoryId != null;

        public bool HasSubCategories => SubCategoriesCount > 0;

        public bool HasAttributes => AttributesCount > 0;
    }
}
