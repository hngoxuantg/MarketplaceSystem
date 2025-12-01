namespace MarketplaceSystem.Web.UI.Admin.Interfaces.IBaseServices
{
    public interface IBaseApiService
    {
        Task<HttpResponseMessage?> GetAsync<TRequest>(
            string endpoint,
            TRequest? request,
            CancellationToken cancellation = default) where TRequest : class;

        Task<HttpResponseMessage?> PostAsync<TRequest>(
            string endpoint,
            TRequest? request,
            CancellationToken cancellation = default) where TRequest : class;

        Task<HttpResponseMessage?> PutAsync<TRequest>(
            string endpoint,
            TRequest? request,
            CancellationToken cancellation = default) where TRequest : class;

        Task<HttpResponseMessage?> PatchAsync<TRequest>(
            string endpoint,
            TRequest? request,
            CancellationToken cancellation = default) where TRequest : class;

        Task<HttpResponseMessage?> DeleteAsync(
            string endpoint,
            CancellationToken cancellation = default);

        Task<HttpResponseMessage?> PostFilesAsync(
            string endpoint,
            List<(Stream Stream, string FileName)> files,
            string formFieldName = "Images",
            Dictionary<string, string>? additionalFields = null,
            CancellationToken cancellation = default);

        Task<HttpResponseMessage?> PostFileAsync(
            string endpoint,
            Stream fileStream,
            string fileName,
            string formFieldName = "icon",
            Dictionary<string, string>? additionalFields = null,
            CancellationToken cancellation = default);
    }
}
