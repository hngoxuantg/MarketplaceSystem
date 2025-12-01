using MarketplaceSystem.Web.UI.Interfaces;
using MarketplaceSystem.Web.UI.Interfaces.IBaseServices;
using MarketplaceSystem.Web.UI.Models.ApiResponses.Common;
using MarketplaceSystem.Web.UI.Models.ViewModels.Auth;
using MarketplaceSystem.Web.UI.Models.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceSystem.Web.UI.Controllers
{
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly IBaseApiService _baseApiService;
        private readonly IEnumService _enumService;

        public AuthController(IAuthService authService, ILogger<AuthController> logger, IBaseApiService baseApiService, IEnumService enumService)
        {
            _authService = authService;
            _logger = logger;
            _baseApiService = baseApiService;
            _enumService = enumService;
        }

        [HttpGet]
        public async Task<IActionResult> Register(CancellationToken cancellation = default)
        {
            var responseLocations = await _enumService.GetVietnamProvincesAsync(cancellation);

            ViewBag.Locations = responseLocations?.Data?.Select(l => new LocationItem
            {
                Id = l.Id,
                Name = l.Name,
            }).ToList() ?? new List<LocationItem>();

            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm, CancellationToken cancellation = default)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                var result = await _authService.RegisterAsync(vm);
                if (!result.Success)
                {
                    ModelState.AddModelError(string.Empty, result.Message ?? "Đăng ký thất bại. Vui lòng thử lại.");
                    if (result.Errors != null)
                    {
                        MapApiErrorsToModelState(result.Errors);
                    }

                    var locations = await _enumService.GetVietnamProvincesAsync(cancellation);

                    ViewBag.Locations = locations?.Data?.Select(l => new LocationItem
                    {
                        Id = l.Id,
                        Name = l.Name,
                    }).ToList() ?? new List<LocationItem>();

                    return View(vm);
                }

                SendOtpViewModel send = new SendOtpViewModel() { Email = vm.Email };
                HttpResponseMessage? response = await _baseApiService.PostAsync("v1/auth/send-otp", send);

                if (response.IsSuccessStatusCode)
                {
                    HttpContext.Session.SetString("EmailVerify", vm.Email);
                    TempData["Success"] = "Đăng ký thành công! Vui lòng kiểm tra email để xác thực tài khoản.";
                    return RedirectToAction("VerifyOtp");
                }

                ViewBag.Error = "Không thể gửi OTP. Vui lòng thử lại.";
                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong quá trình đăng ký cho email {Email}", vm.Email);
                ViewBag.Error = "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau!";
                return View(vm);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (Request.Cookies.ContainsKey("accessToken"))
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                var result = await _authService.LoginAsync(vm);
                if (!result.Success)
                {
                    ModelState.AddModelError(string.Empty, result.Message ?? "Đăng nhập thất bại. Vui lòng thử lại.");
                    if (result.Errors != null)
                    {
                        MapApiErrorsToModelState(result.Errors);
                    }
                    return View(vm);
                }
                _logger.LogInformation("User {Email} logged in successfully", vm.Email);
                TempData["Success"] = "Đăng nhập thành công! Chào mừng bạn trở lại.";

                if (!string.IsNullOrEmpty(vm.ReturnUrl) && Url.IsLocalUrl(vm.ReturnUrl))
                {
                    return Redirect(vm.ReturnUrl);
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error for user {Email}", vm.Email);
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi trong quá trình đăng nhập!");
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("accessToken");
            Response.Cookies.Delete("refreshToken");
            TempData["Success"] = "Đăng xuất thành công! Hẹn gặp lại bạn.";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult LogoutAjax()
        {
            Response.Cookies.Delete("accessToken");
            Response.Cookies.Delete("refreshToken");
            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult VerifyOtp()
        {
            return View(new VerifyOtpViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(VerifyOtpViewModel verifyOtpRequest)
        {
            string? email = HttpContext.Session.GetString("EmailVerify");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Register");

            verifyOtpRequest.Email = email;

            try
            {
                HttpResponseMessage? response = await _baseApiService.PostAsync("v1/auth/verify-otp", verifyOtpRequest);
                var result = await response.Content.ReadFromJsonAsync<ApiResponse>();

                if (result == null || !result.Success)
                {
                    ModelState.AddModelError(string.Empty, result?.Message ?? "Xác minh OTP thất bại. Vui lòng thử lại.");
                    if (result?.Errors != null)
                    {
                        MapApiErrorsToModelState(result.Errors);
                    }
                    return View(verifyOtpRequest);
                }
                else
                {
                    HttpContext.Session.Remove("EmailVerify");
                    TempData["Success"] = "Xác thực email thành công! Vui lòng đăng nhập.";
                    return RedirectToAction("Login");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi trong quá trình xác minh OTP!");
                return View(verifyOtpRequest);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResendOtp()
        {
            string? email = HttpContext.Session.GetString("EmailVerify");
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { success = false, message = "Email không tồn tại trong session" });
            }

            try
            {
                SendOtpViewModel send = new SendOtpViewModel() { Email = email };
                HttpResponseMessage? response = await _baseApiService.PostAsync("v1/auth/send-otp", send);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("OTP resent successfully to {Email}", email);
                    return Json(new { success = true, message = "Mã OTP đã được gửi lại thành công!" });
                }

                return Json(new { success = false, message = "Không thể gửi lại mã OTP. Vui lòng thử lại!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending OTP to {Email}", email);
                return Json(new { success = false, message = "Đã xảy ra lỗi khi gửi lại mã OTP!" });
            }
        }
    }
}