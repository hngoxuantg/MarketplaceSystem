using MarketplaceSystem.Web.UI.Models.ApiResponses;
using MarketplaceSystem.Web.UI.Models.ApiResponses.Common;
using MarketplaceSystem.Web.UI.Models.ViewModels.Auth;

namespace MarketplaceSystem.Web.UI.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse?> GetAccessTokenAsync(CancellationToken cancellation = default);
        Task<AuthResponse> LoginAsync(LoginViewModel loginRequest, CancellationToken cancellation = default);
        Task<ApiResponse> RegisterAsync(RegisterViewModel registerRequest, CancellationToken cancellation = default);
    }
}
