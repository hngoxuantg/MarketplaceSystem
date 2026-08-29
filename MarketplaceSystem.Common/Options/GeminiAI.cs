namespace MarketplaceSystem.Common.Options
{
    public class GeminiAI
    {
        public const string SectionName = "GeminiAI";

        public string ApiKey { get; set; }
        public string BaseUrl { get; set; }
        public string Model { get; set; }
        public int MaxTokens { get; set; }
        public double Temperature { get; set; }
        public double AutoApproveConfidenceThreshold { get; set; }
        public int TimeoutSeconds { get; set; }
        public int MaxRetries { get; set; }
    }
}
