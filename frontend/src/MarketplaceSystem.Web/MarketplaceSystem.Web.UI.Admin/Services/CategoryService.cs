using MarketplaceSystem.Web.UI.Admin.Interfaces;
using MarketplaceSystem.Web.UI.Admin.Interfaces.IBaseServices;
using MarketplaceSystem.Web.UI.Admin.Models.ApiResponses;
using MarketplaceSystem.Web.UI.Admin.Models.ViewModel.Categories;

namespace MarketplaceSystem.Web.UI.Admin.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IBaseApiService _baseService;
        public CategoryService(IBaseApiService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ApiResponse<PaginatedResponse<CategoryViewModel>>?> GetCategoriesAsync(CancellationToken cancellation = default)
        {
            try
            {
                var response = await _baseService.GetAsync<object>("admin/v1/categories", null, cancellation);

                if (response == null) return null;

                // Không dùng cancellation token khi đọc response để tránh TaskCanceledException
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<PaginatedResponse<CategoryViewModel>>>();

                return result;
            }
            catch (TaskCanceledException)
            {
                // Request bị cancel, return null
                return null;
            }
        }

        public async Task<ApiResponse<PaginatedResponse<CategoryViewModel>>?> GetCategoriesAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm,
            bool? isActive,
            CancellationToken cancellation = default)
        {
            try
            {
                var request = new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    SearchTerm = searchTerm,
                    IsActive = isActive
                };

                var response = await _baseService.GetAsync("admin/v1/categories", request, cancellation);

                if (response == null) return null;

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<PaginatedResponse<CategoryViewModel>>>();

                return result;
            }
            catch (TaskCanceledException)
            {
                return null;
            }
        }

        public async Task<ApiResponse<CategoryViewModel>?> GetCategoryByIdAsync(int categoryId, CancellationToken cancellation = default)
        {
            try
            {
                var response = await _baseService.GetAsync<object>($"admin/v1/categories/{categoryId}", null, cancellation);

                if (response == null) return null;

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<CategoryViewModel>>();

                return result;
            }
            catch (TaskCanceledException)
            {
                return null;
            }
        }

        public async Task<ApiResponse<CategoryViewModel>?> CreateCategoryAsync(CreateCategoryViewModel model, CancellationToken cancellation = default)
        {
            try
            {
                var response = await _baseService.PostAsync("admin/v1/categories", model, cancellation);

                if (response == null) return null;

                // Không dùng cancellation token khi đọc response
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<CategoryViewModel>>();

                return result;
            }
            catch (TaskCanceledException)
            {
                return null;
            }
        }

        public async Task<ApiResponse?> UploadCategoryIconAsync(int categoryId, Stream iconStream, string fileName, CancellationToken cancellation = default)
        {
            try
            {
                var response = await _baseService.PostFileAsync(
                    $"admin/v1/files/upload-category-icon/{categoryId}",
                    iconStream,
                    fileName,
                    "icon", // Tên field phải khớp với backend: IFormFile icon
                    null,
                    cancellation
                );

                if (response == null) return null;

                // Không dùng cancellation token khi đọc response
                var result = await response.Content.ReadFromJsonAsync<ApiResponse>();

                return result;
            }
            catch (TaskCanceledException)
            {
                return null;
            }
        }

        public async Task<ApiResponse<CategoryViewModel>?> UpdateCategoryAsync(int categoryId, UpdateCategoryViewModel model, CancellationToken cancellation = default)
        {
            try
            {
                var response = await _baseService.PutAsync($"admin/v1/categories/{categoryId}", model, cancellation);

                if (response == null) return null;

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<CategoryViewModel>>();

                return result;
            }
            catch (TaskCanceledException)
            {
                return null;
            }
        }

        public async Task<ApiResponse?> DeleteCategoryAsync(int categoryId, CancellationToken cancellation = default)
        {
            try
            {
                var response = await _baseService.DeleteAsync($"admin/v1/categories/{categoryId}", cancellation);

                if (response == null) return null;

                var result = await response.Content.ReadFromJsonAsync<ApiResponse>();

                return result;
            }
            catch (TaskCanceledException)
            {
                return null;
            }
        }

        public async Task<ApiResponse?> ToggleActiveAsync(int categoryId, CancellationToken cancellation = default)
        {
            try
            {
                var response = await _baseService.PatchAsync($"admin/v1/categories/{categoryId}/toggle-active", (object?)null, cancellation);

                if (response == null)
                    return new ApiResponse { Success = false, Message = "Không nhận được phản hồi từ server" };

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new ApiResponse
                    {
                        Success = false,
                        Message = $"Lỗi {response.StatusCode}: {errorContent}"
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
                return result ?? new ApiResponse { Success = false, Message = "Không thể đọc dữ liệu phản hồi" };
            }
            catch (TaskCanceledException)
            {
                return new ApiResponse { Success = false, Message = "Request timeout" };
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        public async Task<ApiResponse?> CreateAttributeAsync(int categoryId, CreateAttributeForCategoryViewModel model, CancellationToken cancellation = default)
        {
            try
            {
                var response = await _baseService.PostAsync($"admin/v1/categories/{categoryId}/attributes", model, cancellation);

                if (response == null) return null;

                var result = await response.Content.ReadFromJsonAsync<ApiResponse>();

                return result;
            }
            catch (TaskCanceledException)
            {
                return null;
            }
        }

        public async Task<ApiResponse?> UpdateAttributeAsync(int categoryId, int attributeId, UpdateCategoryAttributeViewModel model, CancellationToken cancellation = default)
        {
            try
            {
                var response = await _baseService.PatchAsync($"admin/v1/categories/{categoryId}/attributes/{attributeId}", model, cancellation);

                if (response == null) return null;

                var result = await response.Content.ReadFromJsonAsync<ApiResponse>();

                return result;
            }
            catch (TaskCanceledException)
            {
                return null;
            }
        }

        public async Task<ApiResponse?> DeleteAttributeAsync(int categoryId, int attributeId, CancellationToken cancellation = default)
        {
            try
            {
                var response = await _baseService.DeleteAsync($"admin/v1/categories/{categoryId}/attributes/{attributeId}", cancellation);

                if (response == null) return null;

                var result = await response.Content.ReadFromJsonAsync<ApiResponse>();

                return result;
            }
            catch (TaskCanceledException)
            {
                return null;
            }
        }

        public async Task<ApiResponse?> UpdateAttributeOptionAsync(int categoryId, int attributeId, int optionId, UpdateAttributeOptionViewModel model, CancellationToken cancellation = default)
        {
            try
            {
                var response = await _baseService.PatchAsync($"admin/v1/categories/{categoryId}/attributes/{attributeId}/options/{optionId}", model, cancellation);

                if (response == null) return null;

                var result = await response.Content.ReadFromJsonAsync<ApiResponse>();

                return result;
            }
            catch (TaskCanceledException)
            {
                return null;
            }
        }

        public async Task<ApiResponse?> DeleteAttributeOptionAsync(int categoryId, int attributeId, int optionId, CancellationToken cancellation = default)
        {
            try
            {
                var response = await _baseService.DeleteAsync($"admin/v1/categories/{categoryId}/attributes/{attributeId}/options/{optionId}", cancellation);

                if (response == null) return null;

                var result = await response.Content.ReadFromJsonAsync<ApiResponse>();

                return result;
            }
            catch (TaskCanceledException)
            {
                return null;
            }
        }
    }
}
