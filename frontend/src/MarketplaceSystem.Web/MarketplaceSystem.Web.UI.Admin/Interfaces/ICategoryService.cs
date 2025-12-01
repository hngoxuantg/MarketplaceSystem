using MarketplaceSystem.Web.UI.Admin.Models.ApiResponses;
using MarketplaceSystem.Web.UI.Admin.Models.ViewModel.Categories;

namespace MarketplaceSystem.Web.UI.Admin.Interfaces
{
    public interface ICategoryService
    {
        Task<ApiResponse<PaginatedResponse<CategoryViewModel>>?> GetCategoriesAsync(CancellationToken cancellation = default);

        Task<ApiResponse<PaginatedResponse<CategoryViewModel>>?> GetCategoriesAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm,
            bool? isActive,
            CancellationToken cancellation = default);

        Task<ApiResponse<CategoryViewModel>?> GetCategoryByIdAsync(int categoryId, CancellationToken cancellation = default);

        Task<ApiResponse<CategoryViewModel>?> CreateCategoryAsync(CreateCategoryViewModel model, CancellationToken cancellation = default);
        Task<ApiResponse?> UploadCategoryIconAsync(int categoryId, Stream iconStream, string fileName, CancellationToken cancellation = default);

        // Update category
        Task<ApiResponse<CategoryViewModel>?> UpdateCategoryAsync(int categoryId, UpdateCategoryViewModel model, CancellationToken cancellation = default);

        // Delete category
        Task<ApiResponse?> DeleteCategoryAsync(int categoryId, CancellationToken cancellation = default);

        // Toggle active status
        Task<ApiResponse?> ToggleActiveAsync(int categoryId, CancellationToken cancellation = default);

        // Attribute management
        Task<ApiResponse?> CreateAttributeAsync(int categoryId, CreateAttributeForCategoryViewModel model, CancellationToken cancellation = default);
        Task<ApiResponse?> UpdateAttributeAsync(int categoryId, int attributeId, UpdateCategoryAttributeViewModel model, CancellationToken cancellation = default);
        Task<ApiResponse?> DeleteAttributeAsync(int categoryId, int attributeId, CancellationToken cancellation = default);

        // Option management
        Task<ApiResponse?> UpdateAttributeOptionAsync(int categoryId, int attributeId, int optionId, UpdateAttributeOptionViewModel model, CancellationToken cancellation = default);
        Task<ApiResponse?> DeleteAttributeOptionAsync(int categoryId, int attributeId, int optionId, CancellationToken cancellation = default);
    }
}
