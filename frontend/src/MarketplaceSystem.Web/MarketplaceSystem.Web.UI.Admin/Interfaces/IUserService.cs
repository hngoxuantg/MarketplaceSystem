using MarketplaceSystem.Web.UI.Admin.Models.ApiResponses;
using MarketplaceSystem.Web.UI.Admin.Models.ViewModel.Users;

namespace MarketplaceSystem.Web.UI.Admin.Interfaces;

public interface IUserService
{
    /// <summary>
    /// Lấy danh sách người dùng với phân trang và tìm kiếm
    /// </summary>
    Task<PaginatedResponse<UserListViewModel>> GetUsersAsync(int pageNumber = 1, int pageSize = 12, string? search = null, int? status = null);

    /// <summary>
    /// Lấy thông tin chi tiết người dùng theo ID
    /// </summary>
    Task<UserDetailViewModel> GetUserByIdAsync(int id);

    /// <summary>
    /// Khóa tài khoản người dùng
    /// </summary>
    Task LockUserAsync(int id, LockUserRequest request);

    /// <summary>
    /// Mở khóa tài khoản người dùng
    /// </summary>
    Task UnlockUserAsync(int id);
}
