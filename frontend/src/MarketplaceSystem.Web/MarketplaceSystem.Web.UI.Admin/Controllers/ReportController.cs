using Microsoft.AspNetCore.Mvc;

namespace MarketplaceSystem.Web.UI.Admin.Controllers
{
    public class ReportController : Controller
    {
        private readonly ILogger<ReportController> _logger;

        public ReportController(ILogger<ReportController> logger)
        {
            _logger = logger;
        }

        // GET: Report - Danh sách báo cáo vi phạm
        public IActionResult Index()
        {
            // TODO: Lấy danh sách báo cáo từ API
            // var reports = await _reportService.GetPendingReportsAsync();
            return View();
        }

        // GET: Report/Details/5 - Chi tiết báo cáo
        public IActionResult Details(int id)
        {
            // TODO: Lấy chi tiết báo cáo từ API
            // var report = await _reportService.GetReportByIdAsync(id);
            ViewBag.ReportId = id;
            return View();
        }

        // POST: Report/Handle - Xử lý báo cáo
        [HttpPost]
        public IActionResult Handle(int id, string action, string reason)
        {
            // TODO: Gọi API để xử lý báo cáo
            // action có thể là: delete_post, ban_user, both, reject
            // await _reportService.HandleReportAsync(id, action, reason);

            string message = action switch
            {
                "delete_post" => "Đã xóa bài đăng và đánh dấu báo cáo đã xử lý",
                "ban_user" => "Đã khóa tài khoản và đánh dấu báo cáo đã xử lý",
                "both" => "Đã xóa bài đăng và khóa tài khoản",
                "reject" => "Đã từ chối báo cáo",
                _ => "Đã xử lý báo cáo"
            };

            return Json(new { success = true, message = message });
        }

        // GET: Report/Processed - Báo cáo đã xử lý
        public IActionResult Processed()
        {
            // TODO: Lấy danh sách báo cáo đã xử lý từ API
            // var reports = await _reportService.GetProcessedReportsAsync();
            return View();
        }
    }
}
