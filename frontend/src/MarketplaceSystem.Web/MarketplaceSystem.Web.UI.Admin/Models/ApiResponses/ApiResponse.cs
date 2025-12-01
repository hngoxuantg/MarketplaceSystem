namespace MarketplaceSystem.Web.UI.Admin.Models.ApiResponses
{
    /// <summary>
    /// Response wrapper từ Backend API
    /// </summary>
    public class ApiResponse<T>
    {
        //Succes == true
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        //Succes == false
        public string Title { get; set; }
        public Dictionary<string, string[]> Errors { get; set; }
        public Error Error { get; set; }
        public string TraceId { get; set; }
        public DateTime Timestamp { get; set; }
    }
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string Title { get; set; }
        public Dictionary<string, string[]> Errors { get; set; }
        public Error Error { get; set; }
        public string TraceId { get; set; }
        public DateTime Timestamp { get; set; }
    }
    public class Error
    {
        public string ErrorCode { get; set; }
        public string Type { get; set; }
    }
}
