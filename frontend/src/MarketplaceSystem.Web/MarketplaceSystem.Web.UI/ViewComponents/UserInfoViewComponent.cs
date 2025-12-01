using MarketplaceSystem.Web.UI.Interfaces;
using MarketplaceSystem.Web.UI.Models.ApiResponses;
using MarketplaceSystem.Web.UI.Models.ViewModels.User;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace MarketplaceSystem.Web.UI.ViewComponents
{
    public class UserInfoViewComponent : ViewComponent
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        public UserInfoViewComponent(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }
        public async Task<IViewComponentResult> InvokeAsync(CancellationToken cancellation = default)
        {
            var token = Request.Cookies["accessToken"];
            var refreshToken = Request.Cookies["refreshToken"];
            string? userId = null;
            if (string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(refreshToken))
            {
                AuthResponse? loginResponse = await _authService.GetAccessTokenAsync(cancellation);
                if (loginResponse == null)
                {
                    return View<UserInfoViewModel>("Default", null);
                }
                token = loginResponse.AccessToken;
            }
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);

                    userId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

                    var result = await _userService.GetUserByIdAsync(int.Parse(userId!), cancellation);

                    if (result?.Data != null)
                    {
                        var viewModel = new UserInfoViewModel
                        {
                            Id = result.Data.Id,
                            FullName = result.Data.FullName,
                            Email = result.Data.Email,
                            Avatar = result.Data.Avatar,
                            Roles = result.Data.Roles
                        };
                        return View("Default", viewModel);
                    }
                }
                catch
                {
                    userId = null;
                    return View<UserInfoViewModel>("Default", null);
                }
            }
            return View<UserInfoViewModel>("Default", null);
        }
    }
}
