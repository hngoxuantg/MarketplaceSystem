using MarketplaceSystem.Web.UI.Interfaces;
using MarketplaceSystem.Web.UI.Interfaces.IBaseServices;
using MarketplaceSystem.Web.UI.Models.ApiResponses.Common;
using MarketplaceSystem.Web.UI.Models.ViewModels.User;

namespace MarketplaceSystem.Web.UI.Services
{
    public class UserService : IUserService
    {
        private readonly IBaseApiService _baseApiService;
        public UserService(IBaseApiService baseApiService)
        {
            _baseApiService = baseApiService;
        }

        public async Task<ApiResponse<UserInfoViewModel>?> GetUserByIdAsync(int id, CancellationToken cancellation = default)
        {
            HttpResponseMessage? response = await _baseApiService.GetAsync<object>(endpoint: $"user/v1/users/{id}",
                    cancellation: cancellation,
                    request: null);

            return await response
                .Content
                .ReadFromJsonAsync<ApiResponse<UserInfoViewModel>>(cancellationToken: cancellation);
        }
    }
}
