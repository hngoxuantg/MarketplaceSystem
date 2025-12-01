using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MarketplaceSystem.Web.UI.Middlewares
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
            try
            {
                var token = context.Request.Cookies["accessToken"];
                if (!string.IsNullOrEmpty(token) && context.User?.Identity?.IsAuthenticated != true)
                {
                    var handler = new JwtSecurityTokenHandler();
                    if (handler.CanReadToken(token))
                    {
                        var jwt = handler.ReadJwtToken(token);
                        var claims = jwt.Claims
                            .Select(c => new Claim(c.Type, c.Value))
                            .ToList();

                        var identity = new ClaimsIdentity(claims, "JwtCookie");
                        context.User = new ClaimsPrincipal(identity);
                    }
                }
            }
            catch
            {
            }

            await _next(context);
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
