using MarketplaceSystem.Web.UI.Admin.Models.ApiResponses;
using MarketplaceSystem.Web.UI.Admin.Models.ViewModel.Shared;

namespace MarketplaceSystem.Web.UI.Admin.Interfaces
{
    public interface IEnumService
    {
        Task<ApiResponse<List<AttributeType>>?> GetAttributeTypesAsync(CancellationToken cancellation = default);

        Task<ApiResponse<List<SortOption>>?> GetSortOptionsAsync(CancellationToken cancellation = default);
    }
}
