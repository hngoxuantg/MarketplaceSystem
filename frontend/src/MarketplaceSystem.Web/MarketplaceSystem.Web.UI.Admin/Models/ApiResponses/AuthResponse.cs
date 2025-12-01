namespace MarketplaceSystem.Web.UI.Admin.Models.ApiResponses
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public Dictionary<string, string[]> Errors { get; set; }
    }
}
