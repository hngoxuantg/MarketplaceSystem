using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Application.Features.Auth.Commands.Login;
using MarketplaceSystem.Application.Features.Auth.Commands.Logout;
using MarketplaceSystem.Application.Features.Auth.Commands.Refresh;
using MarketplaceSystem.Application.Features.Auth.Commands.Register;
using MarketplaceSystem.Application.Features.Auth.Commands.SendOtp;
using MarketplaceSystem.Application.Features.Auth.Commands.VerifyOtp;
using MarketplaceSystem.Common.Models.Responses;
using MarketplaceSystem.Common.Options;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace MarketplaceSystem.API.Controllers.Public.V1
{
    public class AuthController : BaseController
    {
        private readonly AppSettings _appSettings;
        private readonly IMemoryCache _memoryCache;
        private readonly ISender _sender;
        public AuthController(
            IOptions<AppSettings> appSettings,
            IMemoryCache memoryCache,
            ISender mediator)
        {
            _appSettings = appSettings.Value;
            _memoryCache = memoryCache;
            _sender = mediator;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(
            [FromBody] LoginRequest loginRequest,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new LoginCommand("User", loginRequest), cancellationToken);

            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(_appSettings.JwtConfig.RefreshTokenExpirationDays)
            });

            return Ok(new AuthResult
            {
                Success = true,
                Message = "Đăng nhập thành công!",
                AccessToken = result.AccessToken,
                RefreshToken = result.RefreshToken
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync(CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new LogoutCommand(RefreshToken), cancellationToken);

            Response.Cookies.Delete("refreshToken");

            return Ok(new ApiResponse
            {
                Message = "Đăng xuất thành công!",
                Success = true
            });
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshAsync(CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(RefreshToken))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Không tìm thấy RefreshToken ở cookie!"
                });
            }

            var result = await _sender.Send(new RefreshCommand(RefreshToken, "User"), cancellationToken);

            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return Ok(new AuthResult
            {
                Success = true,
                Message = "Token được cập nhật thành công!",
                AccessToken = result.AccessToken,
                RefreshToken = result.RefreshToken
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest registerRequest, CancellationToken cancellation = default)
        {
            await _sender.Send(new RegisterCommand(registerRequest), cancellation);

            var response = new ApiResponse
            {
                Success = true,
                Message = "Đăng ký tài khoản thành công! Vui lòng kiểm tra email để xác nhận mã otp"
            };

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtpAsync([FromBody] SendOtpRequest verifyOtpRequest, CancellationToken cancellation = default)
        {
            if (_memoryCache.TryGetValue($"Email_{verifyOtpRequest.Email}", out _))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng chờ ít phút trước khi gửi lại mã OTP"
                });
            }
            else
            {
                string otp = await _sender.Send(new SendOtpCommand(verifyOtpRequest), cancellation);
                if (otp != null)
                    _memoryCache.Set($"Email_{verifyOtpRequest.Email}", otp, TimeSpan.FromMinutes(5));
            }

            var response = new ApiResponse
            {
                Success = true,
                Message = "Gửi mã OTP thành công! Vui lòng kiểm tra email"
            };

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtpAsync([FromBody] VerifyOtpRequest verifyOtpRequest, CancellationToken cancellation = default)
        {
            if (_memoryCache.TryGetValue($"Email_{verifyOtpRequest.Email}", out string? code))
            {
                if (code != verifyOtpRequest.Otp)
                    throw new ValidatorException(nameof(VerifyOtpRequest.Otp), "Mã otp không hợp lệ, vui lòng thử lại!");
            }
            else
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã OTP đã hết hạn hoặc không tồn tại!"
                });
            }

            _memoryCache.Remove($"Email_{verifyOtpRequest.Email}");

            await _sender.Send(new VerifyOtpCommand(verifyOtpRequest), cancellation);

            var result = new ApiResponse
            {
                Success = true,
                Message = "Xác thực mã OTP thành công!"
            };

            return Ok(result);
        }
    }
}
