using MarketplaceSystem.Web.UI.Admin.Interfaces;
using MarketplaceSystem.Web.UI.Admin.Interfaces.IBaseServices;
using MarketplaceSystem.Web.UI.Admin.Models.ApiResponses;
using MarketplaceSystem.Web.UI.Admin.Models.ViewModel.Shared;

namespace MarketplaceSystem.Web.UI.Admin.Services
{
    public class EnumService : IEnumService
    {
        private readonly IBaseApiService _baseService;
        public EnumService(IBaseApiService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ApiResponse<List<AttributeType>>?> GetAttributeTypesAsync(CancellationToken cancellation = default)
        {
            HttpResponseMessage? response = await _baseService.GetAsync<object>("/v1/enums/category-attribute-types", null, cancellation);

            return await response.Content.ReadFromJsonAsync<ApiResponse<List<AttributeType>>>(cancellationToken: cancellation);
        }

        public async Task<ApiResponse<List<SortOption>>?> GetSortOptionsAsync(CancellationToken cancellation = default)
        {
            HttpResponseMessage? response = await _baseService.GetAsync<object>("admin/v1/enums/sort-options", null, cancellation);

            return await response.Content.ReadFromJsonAsync<ApiResponse<List<SortOption>>>(cancellationToken: cancellation);
        }
    }
}
