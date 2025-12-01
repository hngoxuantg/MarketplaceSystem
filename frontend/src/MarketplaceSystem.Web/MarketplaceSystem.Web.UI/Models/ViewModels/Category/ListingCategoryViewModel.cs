namespace MarketplaceSystem.Web.UI.Models.ViewModels.Category
{
    public class ListingCategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? IconUrl { get; set; }
        public int DisplayOrder { get; set; }
        public List<ListingSubcategoryViewModel>? Subcategories { get; set; }
    }
    public class ListingSubcategoryViewModel
    {
        public int Id { get; set; }
        public int ParentCategoryId { get; set; }
        public string Name { get; set; }
        public string? IconUrl { get; set; }
        public int DisplayOrder { get; set; }
    }
}
