using MarketplaceSystem.Web.UI.Admin.Models.ApiResponses;
using MarketplaceSystem.Web.UI.Admin.Models.ViewModel.Auth;

namespace MarketplaceSystem.Web.UI.Admin.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse?> GetAccessTokenAsync(CancellationToken cancellation = default);
        Task<AuthResponse> LoginAsync(LoginViewModel loginRequest, CancellationToken cancellation = default);
    }
}
