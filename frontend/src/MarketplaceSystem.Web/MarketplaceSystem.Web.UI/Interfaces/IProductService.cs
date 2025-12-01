using MarketplaceSystem.Web.UI.Models.ApiResponses;
using MarketplaceSystem.Web.UI.Models.ApiResponses.Common;
using MarketplaceSystem.Web.UI.Models.ViewModels.Pages;
using MarketplaceSystem.Web.UI.Models.ViewModels.Product;

namespace MarketplaceSystem.Web.UI.Interfaces
{
    public interface IProductService
    {
        Task<ApiResponse<PaginatedResponse<ListProductViewModel>>?> GetProductsAsync(
            ProductFilterViewModel? productFilterViewModel = null,
            CancellationToken cancellation = default);

        Task<ApiResponse<ProductDetailViewModel>?> GetProductByIdAysnc(int id, CancellationToken cancellation = default);

        Task<ApiResponse<ProductDetailViewModel>?> CreateProductAsync(
            CreatePostRequest createPostRequest,
            CancellationToken cancellation = default);

        Task<ApiResponse<object>?> MarkAsSoldAsync(int productId, CancellationToken cancellation = default);

        Task<ApiResponse<object>?> DeleteProductAsync(int productId, CancellationToken cancellation = default);
    }
}
