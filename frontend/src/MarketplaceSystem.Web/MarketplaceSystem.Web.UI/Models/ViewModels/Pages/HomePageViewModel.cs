using MarketplaceSystem.Web.UI.Models.ViewModels.Category;
using MarketplaceSystem.Web.UI.Models.ViewModels.Product;
using MarketplaceSystem.Web.UI.Models.ViewModels.Shared;

namespace MarketplaceSystem.Web.UI.Models.ViewModels.Pages
{
    /// <summary>
    /// ViewModel cho trang chủ - CHỈ chứa data đã xử lý cho View
    /// Kết hợp nhiều ViewModels từ các module khác nhau
    /// </summary>
    public class HomePageViewModel
    {
        public RootCategoryListViewModel? RootCategories { get; set; }
        public List<LocationItem>? Locations { get; set; }
        public ProductListViewModel? Products { get; set; }

        // Helper methods
        public bool HasCategories => RootCategories?.Items?.Any() == true;
        public bool HasLocations => Locations?.Any() == true;
        public bool HasProducts => Products?.Items?.Any() == true;
    }
}
