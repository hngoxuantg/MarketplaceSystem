using MarketplaceSystem.Web.UI.Models.ApiResponses;
using MarketplaceSystem.Web.UI.Models.ApiResponses.Common;

namespace MarketplaceSystem.Web.UI.Interfaces
{
    public interface IEnumService
    {
        Task<ApiResponse<List<EnumsResponse>>?> GetVietnamProvincesAsync(CancellationToken cancellation = default);

        Task<ApiResponse<List<EnumsResponse>>?> GetProductConditionsAsync(CancellationToken cancellation = default);
    }
}
