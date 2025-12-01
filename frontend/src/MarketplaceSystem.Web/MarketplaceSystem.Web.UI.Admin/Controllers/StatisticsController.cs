using MarketplaceSystem.Web.UI.Admin.Interfaces;
using MarketplaceSystem.Web.UI.Admin.Models.ApiResponses;
using MarketplaceSystem.Web.UI.Admin.Models.ViewModel.Statistics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceSystem.Web.UI.Admin.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly ILogger<StatisticsController> _logger;
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(
            ILogger<StatisticsController> logger,
            IStatisticsService statisticsService)
        {
            _logger = logger;
            _statisticsService = statisticsService;
        }

        // GET: Statistics/Posts - Thống kê bài đăng
        public IActionResult Posts()
        {
            var defaultTo = DateTime.UtcNow.Date;
            var defaultFrom = defaultTo.AddDays(-30);

            ViewBag.DefaultFrom = defaultFrom.ToString("yyyy-MM-dd");
            ViewBag.DefaultTo = defaultTo.ToString("yyyy-MM-dd");
            return View();
        }

        // GET: Statistics/Users - Thống kê người dùng
        public IActionResult Users()
        {
            var defaultTo = DateTime.UtcNow.Date;
            var defaultFrom = defaultTo.AddDays(-30);

            ViewBag.DefaultFrom = defaultFrom.ToString("yyyy-MM-dd");
            ViewBag.DefaultTo = defaultTo.ToString("yyyy-MM-dd");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetUserSummary(DateTime? from, DateTime? to, CancellationToken cancellationToken)
        {
            var effectiveTo = to?.Date ?? DateTime.UtcNow.Date;
            var effectiveFrom = from?.Date ?? effectiveTo.AddDays(-30);

            if (effectiveFrom > effectiveTo)
            {
                return BadRequest(new ApiResponse<UserStatisticsViewModel>
                {
                    Success = false,
                    Message = "Từ ngày không được lớn hơn đến ngày."
                });
            }

            var today = DateTime.UtcNow.Date;
            if (effectiveTo > today)
            {
                return BadRequest(new ApiResponse<UserStatisticsViewModel>
                {
                    Success = false,
                    Message = "Ngày kết thúc không được lớn hơn ngày hiện tại!"
                });
            }

            var response = await _statisticsService.GetUserStatisticsAsync(effectiveFrom, effectiveTo, cancellationToken);

            if (response == null)
            {
                return StatusCode(StatusCodes.Status502BadGateway, new ApiResponse<UserStatisticsViewModel>
                {
                    Success = false,
                    Message = "Không nhận được phản hồi từ server"
                });
            }

            if (!response.Success)
            {
                var statusCode = response.Error?.Type == "ValidatorException"
                    ? StatusCodes.Status400BadRequest
                    : StatusCodes.Status500InternalServerError;

                return StatusCode(statusCode, response);
            }

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetPostSummary(
            DateTime? from,
            DateTime? to,
            int? categoryId,
            CancellationToken cancellationToken)
        {
            var effectiveTo = to?.Date ?? DateTime.UtcNow.Date;
            var effectiveFrom = from?.Date ?? effectiveTo.AddDays(-30);

            if (effectiveFrom > effectiveTo)
            {
                return BadRequest(new ApiResponse<PostStatisticsViewModel>
                {
                    Success = false,
                    Message = "Từ ngày không được lớn hơn đến ngày."
                });
            }

            var today = DateTime.UtcNow.Date;
            if (effectiveTo > today)
            {
                return BadRequest(new ApiResponse<PostStatisticsViewModel>
                {
                    Success = false,
                    Message = "Ngày kết thúc không được lớn hơn ngày hiện tại!"
                });
            }

            var response = await _statisticsService.GetPostStatisticsAsync(
                effectiveFrom,
                effectiveTo,
                categoryId,
                cancellationToken);

            if (response == null)
            {
                return StatusCode(StatusCodes.Status502BadGateway, new ApiResponse<PostStatisticsViewModel>
                {
                    Success = false,
                    Message = "Không nhận được phản hồi từ server"
                });
            }

            if (!response.Success)
            {
                var statusCode = response.Error?.Type == "ValidatorException"
                    ? StatusCodes.Status400BadRequest
                    : StatusCodes.Status500InternalServerError;

                return StatusCode(statusCode, response);
            }

            return Ok(response);
        }
    }
}
