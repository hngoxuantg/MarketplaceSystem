namespace MarketplaceSystem.Application.Common.DTOs.Categories
{
    public class ListingCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? IconUrl { get; set; }
        public int DisplayOrder { get; set; }
        public List<ListingSubcategoryDto>? Subcategories { get; set; }
    }
    public class ListingSubcategoryDto
    {
        public int Id { get; set; }
        public int ParentCategoryId { get; set; }
        public string Name { get; set; }
        public string? IconUrl { get; set; }
        public int DisplayOrder { get; set; }
    }
}
