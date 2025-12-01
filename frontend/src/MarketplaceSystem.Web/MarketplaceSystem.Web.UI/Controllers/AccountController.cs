using MarketplaceSystem.Web.UI.Interfaces;
using MarketplaceSystem.Web.UI.Interfaces.IBaseServices;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;

namespace MarketplaceSystem.Web.UI.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IBaseApiService _baseApiService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(
            IUserService userService,
            IBaseApiService baseApiService,
            IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _baseApiService = baseApiService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Profile()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var response = await _userService.GetUserByIdAsync(userId.Value);

            if (response?.Data == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            return View(response.Data);
        }

        [HttpPost]
        public async Task<IActionResult> UploadAvatar(IFormFile avatar)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập!" });
            }

            if (avatar == null || avatar.Length == 0)
            {
                return Json(new { success = false, message = "Vui lòng chọn ảnh!" });
            }

            try
            {
                using var content = new MultipartFormDataContent();
                using var fileStream = avatar.OpenReadStream();
                using var streamContent = new StreamContent(fileStream);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(avatar.ContentType);
                content.Add(streamContent, "avatar", avatar.FileName);

                var response = await _baseApiService.PostAsync(
                    endpoint: $"user/v1/users/{userId.Value}/avatar",
                    request: content);

                // Read raw content to handle both JSON and non-JSON responses
                var raw = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var msg = string.IsNullOrWhiteSpace(raw) ? $"Upload thất bại ({(int)response.StatusCode})." : raw;
                    return Json(new { success = false, message = msg });
                }

                // Try parse as ApiResponse<string>
                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<MarketplaceSystem.Web.UI.Models.ApiResponses.Common.ApiResponse<string>>(raw, options);
                    if (result?.Success == true)
                    {
                        _httpContextAccessor.HttpContext?.Session.SetString("Avatar", result.Data ?? "");
                        return Json(new { success = true, message = result.Message ?? "Cập nhật ảnh đại diện thành công!", avatarUrl = result.Data });
                    }

                    if (result != null)
                    {
                        return Json(new { success = false, message = result.Message ?? "Upload thất bại!" });
                    }
                }
                catch
                {
                    // ignore parse errors, fallback below
                }

                // Fallback: if server returns plain URL or quoted string
                var fallback = raw?.Trim().Trim('"');
                if (!string.IsNullOrWhiteSpace(fallback))
                {
                    _httpContextAccessor.HttpContext?.Session.SetString("Avatar", fallback);
                    return Json(new { success = true, message = "Cập nhật ảnh đại diện thành công!", avatarUrl = fallback });
                }

                return Json(new { success = true, message = "Cập nhật ảnh đại diện thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập!" });
            }

            try
            {
                var response = await _baseApiService.PutAsync(
                    endpoint: $"user/v1/users/{userId.Value}",
                    request: request);

                var result = await response.Content.ReadFromJsonAsync<Models.ApiResponses.Common.ApiResponse<object>>();

                if (result?.Success == true)
                {
                    if (!string.IsNullOrEmpty(request.FullName))
                    {
                        _httpContextAccessor.HttpContext?.Session.SetString("FullName", request.FullName);
                    }

                    return Json(new { success = true, message = "Cập nhật thông tin thành công!" });
                }

                return Json(new { success = false, message = result?.Message ?? "Cập nhật thất bại!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }
    }
    
    public class UpdateProfileRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }
}
