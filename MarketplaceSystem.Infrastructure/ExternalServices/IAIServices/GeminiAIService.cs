using MarketplaceSystem.Application.Common.Interfaces.IExternalServices.IAIServices;
using MarketplaceSystem.Common.Options;
using MarketplaceSystem.Domain.Enums.Business;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MarketplaceSystem.Infrastructure.ExternalServices.IAIServices
{
    public class GeminiAIService : IGeminiAIService
    {
        private readonly HttpClient _httpClient;
        private readonly GeminiAI _options;
        private readonly ILogger<GeminiAIService> _logger;

        public GeminiAIService(
            HttpClient httpClient,
            IOptions<GeminiAI> options,
            ILogger<GeminiAIService> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<(ContentWarningFlag? Flag, string? Detail)> CheckProductContentAsync(
            string title,
            string description,
            List<IFormFile> images,
            CancellationToken cancellation = default)
        {
            try
            {
                string prompt = BuildPrompt(title, description);
                object requestBody = await BuildRequestBodyAsync(prompt, images, cancellation);

                string url = $"{_options.BaseUrl}";

                string jsonContent = JsonSerializer.Serialize(requestBody);
                StringContent httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = httpContent,
                };

                request.Headers.Add("X-goog-api-key", _options.ApiKey);

                HttpResponseMessage response = await _httpClient.SendAsync(request, cancellation);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Gemini API error: {StatusCode} - {Content}",
                        response.StatusCode, errorContent);
                    return (null, null);
                }

                var responseContent = await response.Content.ReadAsStringAsync(cancellation);
                var result = JsonSerializer.Deserialize<GeminiResponse>(responseContent);

                return ParseResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Gemini API for product: {Title}", title);
                return (null, null);
            }
        }

        private string BuildPrompt(string title, string description)
        {
            return $@"Phân tích sản phẩm này để phát hiện vi phạm chính sách.
Kiểm tra:
1. SexualContent - Nội dung khiêu dâm, nhạy cảm
2. Violence - Bạo lực, vũ khí
3. HateSpeech - Kích động thù hận, phân biệt
4. IllegalGoods - Hàng cấm, ma túy

Tiêu đề: {title}
Mô tả: {description}

Trả về JSON:
{{
  ""hasViolation"": true/false,
  ""violationType"": ""SexualContent|Violence|HateSpeech|IllegalGoods|Scam|FakeProduct"",
  ""detail"": ""Giải thích chi tiết bằng tiếng Việt""
}}

Nếu không có vi phạm: hasViolation = false, violationType = null";
        }

        private async Task<object> BuildRequestBodyAsync(string prompt, List<IFormFile> images, CancellationToken cancellation = default)
        {
            List<object> parts = new List<object> { new { text = prompt } };

            foreach (var file in images.Take(5))
            {
                try
                {
                    string base64Image = await ConvertFormFileToBase64Async(file, cancellation);
                    if (!string.IsNullOrEmpty(base64Image))
                    {
                        parts.Add(new
                        {
                            inlineData = new
                            {
                                mimeType = GetMimeType(file),
                                data = base64Image
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to process image: {FileName}", file.FileName);
                }
            }

            return new
            {
                contents = new[] { new { parts = parts.ToArray() } },
                generationConfig = new
                {
                    temperature = 0.2,
                    topK = 1,
                    topP = 1,
                    maxOutputTokens = 500,
                    responseMimeType = "application/json"
                }
            };
        }
        private (ContentWarningFlag? Flag, string? Detail) ParseResponse(GeminiResponse? response)
        {
            if (response?.Candidates == null || !response.Candidates.Any())
                return (null, null);

            var textResponse = response.Candidates[0]?.Content?.Parts?[0]?.Text;
            if (string.IsNullOrEmpty(textResponse))
                return (null, null);

            try
            {
                var analysis = JsonSerializer.Deserialize<ContentAnalysis>(textResponse);

                if (analysis == null || !analysis.HasViolation || string.IsNullOrEmpty(analysis.ViolationType))
                    return (null, null);

                var flag = analysis.ViolationType switch
                {
                    "SexualContent" => ContentWarningFlag.SexualContent,
                    "Violence" => ContentWarningFlag.Violence,
                    "HateSpeech" => ContentWarningFlag.HateSpeech,
                    "IllegalGoods" => ContentWarningFlag.IllegalGoods,
                    _ => (ContentWarningFlag?)null
                };

                return (flag, analysis.Detail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing Gemini response: {Response}", textResponse);
                return (null, null);
            }
        }

        private async Task<string> ConvertFormFileToBase64Async(IFormFile file, CancellationToken cancellation = default)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms, cancellation);
            var bytes = ms.ToArray();
            return Convert.ToBase64String(bytes);
        }
        private string GetMimeType(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".bmp" => "image/bmp",
                _ => throw new InvalidOperationException($"Unsupported image type: {extension}")
            };
        }


        #region Response Models
        private class GeminiResponse
        {
            [JsonPropertyName("candidates")]
            public List<Candidate>? Candidates { get; set; }
        }

        private class Candidate
        {
            [JsonPropertyName("content")]
            public Content? Content { get; set; }
        }

        private class Content
        {
            [JsonPropertyName("parts")]
            public List<Part>? Parts { get; set; }
        }

        private class Part
        {
            [JsonPropertyName("text")]
            public string? Text { get; set; }
        }

        private class ContentAnalysis
        {
            [JsonPropertyName("hasViolation")]
            public bool HasViolation { get; set; }

            [JsonPropertyName("violationType")]
            public string? ViolationType { get; set; }

            [JsonPropertyName("detail")]
            public string? Detail { get; set; }
        }
        #endregion
    }
}