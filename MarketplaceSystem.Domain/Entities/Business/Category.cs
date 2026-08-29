using MarketplaceSystem.Domain.Entities.Base;

namespace MarketplaceSystem.Domain.Entities.Business
{
    public class Category : SoftDeleteEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public int? ParentCategoryId { get; set; }

        public Category? ParentCategory { get; set; }
        public ICollection<Category>? SubCategories { get; set; }


        private readonly List<CategoryAttribute> _categoryAttributes = new List<CategoryAttribute>();
        public IReadOnlyCollection<CategoryAttribute>? CategoryAttributes => _categoryAttributes.AsReadOnly();

        private readonly List<Product> _products = new List<Product>();
        public IReadOnlyCollection<Product>? Products => _products.AsReadOnly();

        public Category() { }
        public Category(
            string name,
            string? description,
            bool isActive,
            int? parentCategoryId)
        {
            Name = name;
            Description = description;
            IsActive = isActive;
            ParentCategoryId = parentCategoryId;
        }
        public void AddCategoryAttribute(CategoryAttribute categoryAttribute)
        {
            _categoryAttributes.Add(categoryAttribute);
        }

        public void ToggleActive()
        {
            if (IsActive)
            {
                IsActive = false;
            }
            else
            {
                IsActive = true;
            }
        }
    }
}
