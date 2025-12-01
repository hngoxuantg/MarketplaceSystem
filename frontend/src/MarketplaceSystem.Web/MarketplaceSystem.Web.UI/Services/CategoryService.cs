using MarketplaceSystem.Web.UI.Interfaces;
using MarketplaceSystem.Web.UI.Interfaces.IBaseServices;
using MarketplaceSystem.Web.UI.Models.ApiResponses.Common;
using MarketplaceSystem.Web.UI.Models.ViewModels.Category;

namespace MarketplaceSystem.Web.UI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IBaseApiService _baseApiService;
        public CategoryService(IBaseApiService baseApiService)
        {
            _baseApiService = baseApiService;
        }
        public async Task<ApiResponse<RootCategoryListViewModel>?> GetHomePageCategoriesAsync(CancellationToken cancellation = default)
        {
            HttpResponseMessage? response = await _baseApiService.GetAsync<object>(
                endpoint: "v1/categories/home",
                request: null,
                cancellation: cancellation
            );
            return response.Content
                .ReadFromJsonAsync<ApiResponse<RootCategoryListViewModel>>(cancellationToken: cancellation).Result;
        }
        public async Task<ApiResponse<List<ListingCategoryViewModel>>?> GetListingCategoriesAsync(CancellationToken cancellation = default)
        {
            HttpResponseMessage? response = await _baseApiService.GetAsync<object>(
                endpoint: "user/v1/categories/listing-categories",
                request: null,
                cancellation: cancellation);

            return await response.Content
                .ReadFromJsonAsync<ApiResponse<List<ListingCategoryViewModel>>>(cancellation);
        }
        public async Task<ApiResponse<List<CategoryAttributeViewModel>>?> GetCategoryAttributesAsync(int categoryId, CancellationToken cancellation = default)
        {
            HttpResponseMessage? response = await _baseApiService.GetAsync<object>(
                endpoint: $"user/v1/categories/{categoryId}/attributes",
                request: null,
                cancellation: cancellation);

            return await response.Content
                .ReadFromJsonAsync<ApiResponse<List<CategoryAttributeViewModel>>>(cancellation);
        }
    }
}
