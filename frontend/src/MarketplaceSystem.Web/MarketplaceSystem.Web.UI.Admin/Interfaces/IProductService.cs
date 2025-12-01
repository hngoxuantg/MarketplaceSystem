using MarketplaceSystem.Web.UI.Admin.Models.ApiResponses;
using MarketplaceSystem.Web.UI.Admin.Models.ViewModel.Products;

namespace MarketplaceSystem.Web.UI.Admin.Interfaces
{
    public interface IProductService
    {
        Task<ApiResponse<PaginatedResponse<ProductListViewModel>>?> GetProductsAsync(
            ProductFilterViewModel filter,
            CancellationToken cancellation = default);

        Task<ApiResponse?> ApproveProductAsync(int productId, string? note, CancellationToken cancellation = default);

        Task<ApiResponse?> RejectProductAsync(int productId, string reason, CancellationToken cancellation = default);

        Task<ApiResponse?> DeleteProductAsync(int productId, string reason, CancellationToken cancellation = default);
    }
}
