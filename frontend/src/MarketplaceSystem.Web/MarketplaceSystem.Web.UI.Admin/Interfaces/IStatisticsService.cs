using MarketplaceSystem.Web.UI.Admin.Models.ApiResponses;
using MarketplaceSystem.Web.UI.Admin.Models.ViewModel.Statistics;

namespace MarketplaceSystem.Web.UI.Admin.Interfaces
{
    public interface IStatisticsService
    {
        Task<ApiResponse<PostStatisticsViewModel>?> GetPostStatisticsAsync(
            DateTime? startDate = null, 
            DateTime? endDate = null, 
            int? categoryId = null,
            CancellationToken cancellation = default);

        Task<ApiResponse<UserStatisticsViewModel>?> GetUserStatisticsAsync(
            DateTime? startDate = null, 
            DateTime? endDate = null,
            CancellationToken cancellation = default);
    }
}
