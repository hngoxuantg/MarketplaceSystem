using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace MarketplaceSystem.Web.UI.Handlers
{
    public class LoggingHandler : DelegatingHandler
    {
        private readonly ILogger<LoggingHandler> _logger;

        public LoggingHandler(ILogger<LoggingHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Request: {Method} {Url}", request.Method, request.RequestUri);

            if (request.Content != null)
            {
                string body = await request.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogDebug("Request body: {Body}", body);
            }

            var stopwatch = Stopwatch.StartNew();
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            stopwatch.Stop();

            string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            string prettyBody = responseBody;
            try
            {
                using var doc = JsonDocument.Parse(responseBody);
                prettyBody = JsonSerializer.Serialize(doc, new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                });
            }
            catch
            {
            }

            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
            {
                _logger.LogInformation("Response: {StatusCode} in {Elapsed} ms\nBody:\n{Body}",
                    response.StatusCode, stopwatch.ElapsedMilliseconds, prettyBody);
            }
            else if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
            {
                _logger.LogWarning("Client error: {StatusCode} in {Elapsed} ms\nBody:\n{Body}",
                    response.StatusCode, stopwatch.ElapsedMilliseconds, prettyBody);
            }
            else if ((int)response.StatusCode >= 500)
            {
                _logger.LogError("Server error: {StatusCode} in {Elapsed} ms\nBody:\n{Body}",
                    response.StatusCode, stopwatch.ElapsedMilliseconds, prettyBody);
            }
            else
            {
                _logger.LogInformation("Unexpected response: {StatusCode} in {Elapsed} ms\nBody:\n{Body}",
                    response.StatusCode, stopwatch.ElapsedMilliseconds, prettyBody);
            }

            return response;
        }

    }
    public static class LoggingHandlerExtensions
    {
        public static IHttpClientBuilder AddLoggingHandler(this IHttpClientBuilder builder)
        {
            return builder.AddHttpMessageHandler<LoggingHandler>();
        }
    }
}
