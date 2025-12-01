using MarketplaceSystem.Web.UI.Admin.Interfaces;
using MarketplaceSystem.Web.UI.Admin.Models.ApiResponses;
using MarketplaceSystem.Web.UI.Admin.Models.ViewModel.Users;
using MarketplaceSystem.Web.UI.Admin.Services.BaseServices;
using System.Net.Http.Json;

namespace MarketplaceSystem.Web.UI.Admin.Services;

public class UserService : BaseApiService, IUserService
{
    private const string BaseEndpoint = "/api/admin/v1/users";

    public UserService(IHttpClientFactory httpClient) : base(httpClient)
    {
    }

    public async Task<PaginatedResponse<UserListViewModel>> GetUsersAsync(
        int pageNumber = 1, 
        int pageSize = 12, 
        string? search = null, 
        int? status = null)
    {
        var request = new
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Search = search,
            Status = status
        };

        var response = await GetAsync(BaseEndpoint, request);

        if (response == null || !response.IsSuccessStatusCode)
        {
            throw new Exception("Không thể lấy danh sách người dùng");
        }

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<PaginatedResponse<UserListViewModel>>>();

        if (result?.Data == null)
        {
            throw new Exception("Dữ liệu trả về không hợp lệ");
        }

        return result.Data;
    }

    public async Task<UserDetailViewModel> GetUserByIdAsync(int id)
    {
        var endpoint = $"{BaseEndpoint}/{id}";
        var response = await GetAsync<object>(endpoint);

        if (response == null || !response.IsSuccessStatusCode)
        {
            if (response?.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new Exception("Không tìm thấy người dùng");
            }
            throw new Exception("Không thể lấy thông tin người dùng");
        }

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserDetailViewModel>>();

        if (result?.Data == null)
        {
            throw new Exception("Dữ liệu trả về không hợp lệ");
        }

        return result.Data;
    }

    public async Task LockUserAsync(int id, LockUserRequest request)
    {
        var endpoint = $"{BaseEndpoint}/{id}/lock";
        var response = await PostAsync(endpoint, request);

        if (response == null || !response.IsSuccessStatusCode)
        {
            var errorContent = await response!.Content.ReadAsStringAsync();
            throw new Exception($"Không thể khóa người dùng: {errorContent}");
        }
    }

    public async Task UnlockUserAsync(int id)
    {
        var endpoint = $"{BaseEndpoint}/{id}/unlock";
        var response = await PostAsync<object>(endpoint);

        if (response == null || !response.IsSuccessStatusCode)
        {
            var errorContent = await response!.Content.ReadAsStringAsync();
            throw new Exception($"Không thể mở khóa người dùng: {errorContent}");
        }
    }
}
