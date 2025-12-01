namespace MarketplaceSystem.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int? ParentCategoryId { get; set; }

        public int? DisplayOrder { get; set; }

        public bool IsActive { get; set; }

        public List<CreateCategoryAttributeRequest>? Attributes { get; set; }
    }
}
