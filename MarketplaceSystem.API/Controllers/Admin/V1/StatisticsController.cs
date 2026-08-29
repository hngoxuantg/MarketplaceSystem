using MarketplaceSystem.Application.Common.DTOs.Statistics.Products;
using MarketplaceSystem.Application.Common.DTOs.Statistics.Users;
using MarketplaceSystem.Application.Features.Statistics.Products.Queries.GetSummary;
using MarketplaceSystem.Application.Features.Statistics.Users.Queries.GetSummary;
using MarketplaceSystem.Common.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceSystem.API.Controllers.Admin.V1
{
    public class StatisticsController : BaseController
    {
        private readonly ISender _sender;
        public StatisticsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("users/summary")]
        public async Task<IActionResult> GetUserSummaryAsync([FromQuery] GetSummaryRequest request, CancellationToken cancellation = default)
        {
            var result = await _sender.Send(new GetSummaryQuery(request), cancellation);

            return Ok(new ApiResponse<SummaryDto>
            {
                Success = true,
                Message = "Lấy thống kê tổng quan người dùng thành công",
                Data = result
            });
        }

        [HttpGet("products/summary")]
        public async Task<IActionResult> GetProductSummaryAsync([FromQuery] GetProductSummaryRequest request, CancellationToken cancellation = default)
        {
            var result = await _sender.Send(new GetProductSummaryQuery(request), cancellation);

            return Ok(new ApiResponse<ProductSummaryDto>
            {
                Success = true,
                Message = "Lấy thống kê tổng quan bài đăng thành công",
                Data = result
            });
        }
    }
}
