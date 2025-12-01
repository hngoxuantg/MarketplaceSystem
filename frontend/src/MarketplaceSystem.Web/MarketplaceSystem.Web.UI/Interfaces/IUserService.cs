using MarketplaceSystem.Web.UI.Models.ApiResponses.Common;
using MarketplaceSystem.Web.UI.Models.ViewModels.User;

namespace MarketplaceSystem.Web.UI.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<UserInfoViewModel>?> GetUserByIdAsync(int id, CancellationToken cancellation = default);
    }
}
