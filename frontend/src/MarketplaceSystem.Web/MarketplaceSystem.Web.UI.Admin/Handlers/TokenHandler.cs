using MarketplaceSystem.Web.UI.Admin.Models.ApiResponses;
using System.Net;
using System.Net.Http.Headers;

namespace MarketplaceSystem.Web.UI.Admin.Handlers
{
    public class TokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<TokenHandler> _logger;

        public TokenHandler(
            IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory httpClientFactory,
            ILogger<TokenHandler> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor?.HttpContext;
            if (httpContext == null)
                return await base.SendAsync(request, cancellationToken);

            var accessToken = httpContext.Request.Cookies["accessToken"];

            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var refreshToken = httpContext.Request.Cookies["refreshToken"];

                if (!string.IsNullOrEmpty(refreshToken))
                {
                    _logger.LogInformation("Received 401, attempting token refresh");

                    var newAccessToken = await RefreshTokenAsync(refreshToken, cancellationToken);

                    if (!string.IsNullOrEmpty(newAccessToken))
                    {
                        request.Headers.Authorization =
                            new AuthenticationHeaderValue("Bearer", newAccessToken);

                        response.Dispose();
                        response = await base.SendAsync(request, cancellationToken);

                        _logger.LogInformation("Request retried successfully with new token");
                    }
                    else
                    {
                        _logger.LogWarning("Token refresh failed, throwing UnauthorizedException");
                        httpContext.Response.Cookies.Delete("accessToken");
                        httpContext.Response.Cookies.Delete("refreshToken");
                        throw new UnauthorizedAccessException("Session expired. Please login again.");
                    }
                }
                else
                {
                    _logger.LogWarning("No refresh token found, throwing UnauthorizedException");
                    httpContext.Response.Cookies.Delete("accessToken");
                    throw new UnauthorizedAccessException("Authentication required. Please login.");
                }
            }

            return response;
        }

        private async Task<string?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor?.HttpContext;
            if (httpContext == null) return null;

            try
            {
                var client = _httpClientFactory.CreateClient("ApiClients");

                var request = new HttpRequestMessage(HttpMethod.Post, "v1/admin/auth/refresh-token");
                request.Headers.Add("Cookie", $"refreshToken={refreshToken}");

                var response = await client.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken);

                    if (apiResponse != null && !string.IsNullOrEmpty(apiResponse.AccessToken))
                    {
                        httpContext.Response.Cookies.Append("accessToken", apiResponse.AccessToken, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTimeOffset.UtcNow.AddMinutes(15)
                        });

                        if (!string.IsNullOrEmpty(apiResponse.RefreshToken))
                        {
                            httpContext.Response.Cookies.Append("refreshToken", apiResponse.RefreshToken, new CookieOptions
                            {
                                HttpOnly = true,
                                Secure = true,
                                SameSite = SameSiteMode.Strict,
                                Expires = DateTimeOffset.UtcNow.AddDays(7)
                            });
                        }

                        _logger.LogInformation("Token refreshed successfully");
                        return apiResponse.AccessToken;
                    }
                }
                else
                {
                    _logger.LogError("Token refresh failed with status: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
            }

            return null;
        }
    }
}