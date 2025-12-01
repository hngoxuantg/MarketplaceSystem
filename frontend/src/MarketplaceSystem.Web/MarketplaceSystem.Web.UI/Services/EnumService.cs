using MarketplaceSystem.Web.UI.Interfaces;
using MarketplaceSystem.Web.UI.Interfaces.IBaseServices;
using MarketplaceSystem.Web.UI.Models.ApiResponses;
using MarketplaceSystem.Web.UI.Models.ApiResponses.Common;

namespace MarketplaceSystem.Web.UI.Services
{
    public class EnumService : IEnumService
    {
        private readonly IBaseApiService _baseApiService;
        public EnumService(IBaseApiService baseApiService)
        {
            _baseApiService = baseApiService;
        }
        public async Task<ApiResponse<List<EnumsResponse>>?> GetVietnamProvincesAsync(CancellationToken cancellation = default)
        {
            HttpResponseMessage? response = await _baseApiService.GetAsync<object>(
                endpoint: "v1/enums/vietnam-provinces",
                request: null,
                cancellation: cancellation
            );
            return await response.Content
                .ReadFromJsonAsync<ApiResponse<List<EnumsResponse>>>(cancellationToken: cancellation);
        }
        public async Task<ApiResponse<List<EnumsResponse>>?> GetProductConditionsAsync(CancellationToken cancellation = default)
        {
            HttpResponseMessage? response = await _baseApiService.GetAsync<object>(
                endpoint: "v1/enums/product-conditions",
                request: null,
                cancellation: cancellation);

            return await response.Content
                .ReadFromJsonAsync<ApiResponse<List<EnumsResponse>>>(cancellation);
        }
    }
}
