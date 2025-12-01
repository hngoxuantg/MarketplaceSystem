using MarketplaceSystem.Web.UI.Extensions;
using MarketplaceSystem.Web.UI.Interfaces.IBaseServices;
using System.Text;
using System.Text.Json;

namespace MarketplaceSystem.Web.UI.Services.BaseServices
{
    public class BaseApiService : IBaseApiService
    {
        protected readonly HttpClient _httpClient;


        public BaseApiService(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient.CreateClient("ApiClients");
        }

        public virtual async Task<HttpResponseMessage?> GetAsync<TRequest>(
            string endpoint,
            TRequest? request = null,
            CancellationToken cancellation = default) where TRequest : class
        {
            Dictionary<string, string>? queryParams;

            if (request != null)
            {
                queryParams = request.ToDictionaryString();
            }
            else
            {
                queryParams = new Dictionary<string, string>();
            }

            if (queryParams.Any())
            {
                var queryString = await new FormUrlEncodedContent(queryParams).ReadAsStringAsync();
                endpoint = $"{endpoint}?{queryString}";
            }

            return await _httpClient.GetAsync(endpoint);
        }
        public virtual async Task<HttpResponseMessage?> PostAsync<TRequest>(
            string endpoint,
            TRequest? request = null,
            CancellationToken cancellation = default) where TRequest : class
        {
            HttpResponseMessage response;

            if (request == null)
            {
                response = await _httpClient.PostAsync(endpoint, null, cancellation);
            }
            else
            {
                string json = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                response = await _httpClient.PostAsync(endpoint, content, cancellation);
            }

            return response;
        }
        public virtual async Task<HttpResponseMessage?> PostFilesAsync(
            string endpoint,
            List<(Stream Stream, string FileName)> files,
            string formFieldName = "Images",
            Dictionary<string, string>? additionalFields = null,
            CancellationToken cancellation = default)
        {
            using var form = new MultipartFormDataContent();

            foreach (var file in files)
            {
                var fileContent = new StreamContent(file.Stream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                form.Add(fileContent, formFieldName, file.FileName);
            }

            if (additionalFields != null)
            {
                foreach (var kvp in additionalFields)
                {
                    form.Add(new StringContent(kvp.Value), kvp.Key);
                }
            }

            return await _httpClient.PostAsync(endpoint, form, cancellation);
        }

        public virtual async Task<HttpResponseMessage?> PutAsync<TRequest>(
            string endpoint,
            TRequest? request = null,
            CancellationToken cancellation = default) where TRequest : class
        {
            HttpResponseMessage response;
            if (request == null)
            {
                response = await _httpClient.PutAsync(endpoint, null, cancellation);
            }
            else
            {
                string json = JsonSerializer.Serialize(request);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                response = await _httpClient.PutAsync(endpoint, content, cancellation);
            }
            return response;
        }

        public virtual async Task<HttpResponseMessage?> PatchAsync<TRequest>(
            string endpoint,
            TRequest? request = null,
            CancellationToken cancellation = default) where TRequest : class
        {
            HttpResponseMessage response;
            if (request == null)
            {
                response = await _httpClient.PatchAsync(endpoint, null, cancellation);
            }
            else
            {
                string json = JsonSerializer.Serialize(request);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                response = await _httpClient.PatchAsync(endpoint, content, cancellation);
            }
            return response;
        }

        public virtual async Task<HttpResponseMessage?> DeleteAsync(
            string endpoint,
            CancellationToken cancellation = default)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync(endpoint, cancellation);

            return response;
        }
    }
}
