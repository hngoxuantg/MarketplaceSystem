using MarketplaceSystem.Web.UI.Models.ApiResponses.Common;
using MarketplaceSystem.Web.UI.Models.ViewModels.Category;

namespace MarketplaceSystem.Web.UI.Interfaces
{
    public interface ICategoryService
    {
        Task<ApiResponse<RootCategoryListViewModel>?> GetHomePageCategoriesAsync(CancellationToken cancellation = default);

        Task<ApiResponse<List<ListingCategoryViewModel>>?> GetListingCategoriesAsync(CancellationToken cancellation = default);

        Task<ApiResponse<List<CategoryAttributeViewModel>>?> GetCategoryAttributesAsync(int categoryId, CancellationToken cancellation = default);
    }
}
