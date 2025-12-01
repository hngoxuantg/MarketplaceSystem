using Microsoft.AspNetCore.Mvc;

namespace MarketplaceSystem.Web.UI.Admin.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ILogger<SettingsController> _logger;

        public SettingsController(ILogger<SettingsController> logger)
        {
            _logger = logger;
        }

        // GET: Settings - Cài đặt hệ thống
        public IActionResult Index()
        {
            // TODO: Lấy cài đặt từ API
            // var settings = await _settingsService.GetSettingsAsync();
            return View();
        }

        // POST: Settings/Update - Cập nhật cài đặt
        [HttpPost]
        public IActionResult Update(Dictionary<string, string> settings)
        {
            // TODO: Gọi API để cập nhật cài đặt
            // await _settingsService.UpdateSettingsAsync(settings);
            return Json(new { success = true, message = "Đã cập nhật cài đặt!" });
        }
    }
}
