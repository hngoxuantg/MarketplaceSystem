using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MarketplaceSystem.Web.UI.Admin.Middlewares
{
    public class JwtCookieAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtCookieAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var accessToken = context.Request.Cookies["accessToken"];
            var refreshToken = context.Request.Cookies["refreshToken"];

            if (context.Request.Path.StartsWithSegments("/auth/login") ||
                context.Request.Path.StartsWithSegments("/auth/refresh") ||
                context.Request.Path.StartsWithSegments("/auth/logout"))
            {
                await _next(context);
                return;
            }

            if (string.IsNullOrEmpty(accessToken) && string.IsNullOrEmpty(refreshToken))
            {
                context.Response.Redirect("/auth/login");
                return;
            }

            if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
            {
                SetUserFromJwt(context, accessToken);
                await _next(context);
                return;
            }

            if (!string.IsNullOrEmpty(accessToken) && string.IsNullOrEmpty(refreshToken))
            {
                context.Response.Redirect("/auth/login");
                return;
            }

            if (string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
            {
                await _next(context);
                return;
            }
        }

        private void SetUserFromJwt(HttpContext context, string token)
        {
            var handler = new JwtSecurityTokenHandler();
            if (handler.CanReadToken(token))
            {
                var jwt = handler.ReadJwtToken(token);
                var claims = jwt.Claims.Select(c => new Claim(c.Type, c.Value)).ToList();
                var identity = new ClaimsIdentity(claims, "JwtCookie");
                context.User = new ClaimsPrincipal(identity);
            }
        }
    }

    public static class JwtCookieAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtCookieAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtCookieAuthenticationMiddleware>();
        }
    }
}
