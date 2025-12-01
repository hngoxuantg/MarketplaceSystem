namespace MarketplaceSystem.Application.Features.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int? ParentCategoryId { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
    }
}
