using MarketplaceSystem.Web.UI.Admin.Interfaces;
using MarketplaceSystem.Web.UI.Admin.Interfaces.IBaseServices;
using MarketplaceSystem.Web.UI.Admin.Models.ApiResponses;
using MarketplaceSystem.Web.UI.Admin.Models.ViewModel.Statistics;
using System.Net.Http.Json;

namespace MarketplaceSystem.Web.UI.Admin.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IBaseApiService _baseService;

        public StatisticsService(IBaseApiService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ApiResponse<PostStatisticsViewModel>?> GetPostStatisticsAsync(
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? categoryId = null,
            CancellationToken cancellation = default)
        {
            try
            {
                var queryParams = new
                {
                    From = startDate?.ToString("yyyy-MM-dd"),
                    To = endDate?.ToString("yyyy-MM-dd"),
                    CategoryId = categoryId
                };

                var response = await _baseService.GetAsync<object>("admin/v1/statistics/products/summary", queryParams, cancellation);

                if (response == null)
                {
                    return new ApiResponse<PostStatisticsViewModel> 
                    { 
                        Success = false, 
                        Message = "Không nhận được phản hồi từ server" 
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<PostStatisticsViewModel>>();

                if (result != null)
                {
                    return result;
                }

                return new ApiResponse<PostStatisticsViewModel>
                {
                    Success = false,
                    Message = "Không thể phân tích dữ liệu thống kê bài đăng."
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PostStatisticsViewModel> 
                { 
                    Success = false, 
                    Message = $"Lỗi: {ex.Message}" 
                };
            }
        }

        public async Task<ApiResponse<UserStatisticsViewModel>?> GetUserStatisticsAsync(
            DateTime? startDate = null,
            DateTime? endDate = null,
            CancellationToken cancellation = default)
        {
            try
            {
                var queryParams = new
                {
                    From = startDate?.ToString("yyyy-MM-dd"),
                    To = endDate?.ToString("yyyy-MM-dd")
                };

                var response = await _baseService.GetAsync<object>("admin/v1/statistics/users/summary", queryParams, cancellation);

                if (response == null)
                {
                    return new ApiResponse<UserStatisticsViewModel> 
                    { 
                        Success = false, 
                        Message = "Không nhận được phản hồi từ server" 
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserStatisticsViewModel>>();

                if (result != null)
                {
                    return result;
                }

                return new ApiResponse<UserStatisticsViewModel>
                {
                    Success = false,
                    Message = "Không thể phân tích dữ liệu thống kê người dùng."
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserStatisticsViewModel> 
                { 
                    Success = false, 
                    Message = $"Lỗi: {ex.Message}" 
                };
            }
        }
    }
}
