using MarketplaceSystem.Web.UI.Admin.Interfaces;
using MarketplaceSystem.Web.UI.Admin.Interfaces.IBaseServices;
using MarketplaceSystem.Web.UI.Admin.Models.ApiResponses;
using MarketplaceSystem.Web.UI.Admin.Models.ViewModel.Products;

namespace MarketplaceSystem.Web.UI.Admin.Services
{
    public class ProductService : IProductService
    {
        private readonly IBaseApiService _baseService;

        public ProductService(IBaseApiService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ApiResponse<PaginatedResponse<ProductListViewModel>>?> GetProductsAsync(
            ProductFilterViewModel filter,
            CancellationToken cancellation = default)
        {
            try
            {
                var queryParams = new
                {
                    Status = filter.Status,
                    CategoryId = filter.CategoryId,
                    SortBy = filter.SortBy,
                    Search = filter.Search,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize
                };

                var response = await _baseService.GetAsync("admin/v1/products", queryParams, cancellation);

                if (response == null) return null;

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<PaginatedResponse<ProductListViewModel>>>();

                return result;
            }
            catch (TaskCanceledException)
            {
                return null;
            }
        }

        public async Task<ApiResponse?> ApproveProductAsync(int productId, string? note, CancellationToken cancellation = default)
        {
            try
            {
                var data = note != null ? new { Note = note } : null;
                var response = await _baseService.PostAsync($"admin/v1/products/{productId}/approve", data, cancellation);

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

        public async Task<ApiResponse?> RejectProductAsync(int productId, string reason, CancellationToken cancellation = default)
        {
            try
            {
                var data = new { RejectionReason = reason };
                var response = await _baseService.PostAsync($"admin/v1/products/{productId}/reject", data, cancellation);

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

        public async Task<ApiResponse?> DeleteProductAsync(int productId, string reason, CancellationToken cancellation = default)
        {
            try
            {
                // Gọi DELETE không cần reason parameter
                var response = await _baseService.DeleteAsync($"admin/v1/products/{productId}", cancellation);

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
    }
}
