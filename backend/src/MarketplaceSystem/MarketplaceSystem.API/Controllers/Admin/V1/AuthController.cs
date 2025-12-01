using MarketplaceSystem.Application.Features.Auth.Commands.Login;
using MarketplaceSystem.Application.Features.Auth.Commands.Logout;
using MarketplaceSystem.Application.Features.Auth.Commands.Refresh;
using MarketplaceSystem.Common.Models.Responses;
using MarketplaceSystem.Common.Options;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MarketplaceSystem.API.Controllers.Admin.V1
{
    public class AuthController : BaseController
    {
        private readonly AppSettings _appSettings;
        private readonly ISender _sender;
        public AuthController(
            IOptions<AppSettings> appSettings,
            ISender mediator)
        {
            _appSettings = appSettings.Value;
            _sender = mediator;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(
            [FromBody] LoginRequest loginRequest,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new LoginCommand("Admin", loginRequest), cancellationToken);

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
            await _sender.Send(new LogoutCommand(RefreshToken), cancellationToken);

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

            var result = await _sender.Send(new RefreshCommand(RefreshToken, "Admin"), cancellationToken);

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
    }
}
