using MarketplaceSystem.Web.UI.Interfaces;
using MarketplaceSystem.Web.UI.Models.ApiResponses;
using MarketplaceSystem.Web.UI.Models.ApiResponses.Common;
using MarketplaceSystem.Web.UI.Models.ViewModels.Auth;
using System.Text;
using System.Text.Json;

namespace MarketplaceSystem.Web.UI.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IHttpClientFactory httpClient, ILogger<AuthService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient.CreateClient("ApiClients");
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<AuthResponse?> GetAccessTokenAsync(CancellationToken cancellation = default)
        {
            try
            {
                string refreshToken = _httpContextAccessor?.HttpContext?.Request.Cookies["refreshToken"] ?? string.Empty;

                var request = new HttpRequestMessage(HttpMethod.Post, "v1/auth/refresh-token");
                request.Headers.Add("Cookie", $"refreshToken={refreshToken}");

                HttpResponseMessage response = await _httpClient.SendAsync(request, cancellation);
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<AuthResponse>(cancellation);
                    return apiResponse;
                }
                else
                {
                    _logger.LogError("Failed to retrieve access token. Status Code: {StatusCode}", response.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving access token.");
                return null;
            }
        }

        public async Task<AuthResponse> LoginAsync(LoginViewModel loginRequest, CancellationToken cancellation = default)
        {
            try
            {
                string json = JsonSerializer.Serialize(loginRequest);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync("v1/auth/login", content, cancellation);

                var apiResponse = await response.Content.ReadFromJsonAsync<AuthResponse>(cancellation);

                if (response.IsSuccessStatusCode && apiResponse?.Success == true && apiResponse != null)
                {
                    if (!string.IsNullOrEmpty(apiResponse.AccessToken))
                    {
                        _httpContextAccessor?.HttpContext?.Response.Cookies.Append("accessToken", apiResponse.AccessToken, new CookieOptions
                        {
                            HttpOnly = false,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTimeOffset.UtcNow.AddMinutes(15)
                        });

                        _httpContextAccessor?.HttpContext?.Response.Cookies.Append("refreshToken", apiResponse.RefreshToken, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTimeOffset.UtcNow.AddDays(7)
                        });
                    }
                }

                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during login.");
                return null;
            }
        }

        public async Task<ApiResponse> RegisterAsync(RegisterViewModel registerRequest, CancellationToken cancellation = default)
        {
            try
            {
                string json = JsonSerializer.Serialize(registerRequest);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync("v1/auth/register", content, cancellation);

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>(cancellation);

                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during registration.");
                return null;
            }
        }
    }
}
