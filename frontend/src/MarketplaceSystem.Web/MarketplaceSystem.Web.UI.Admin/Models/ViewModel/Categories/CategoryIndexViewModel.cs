using MarketplaceSystem.Web.UI.Admin.Models.ApiResponses;

namespace MarketplaceSystem.Web.UI.Admin.Models.ViewModel.Categories
{
    public class CategoryIndexViewModel
    {
        public PaginatedResponse<CategoryViewModel> Categories { get; set; } = new();

        // Statistics
        public int TotalCategories { get; set; }
        public int ActiveCategories { get; set; }
        public int InactiveCategories { get; set; }
        public int TotalPosts { get; set; }

        // Filter
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}
