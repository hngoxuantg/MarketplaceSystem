using MarketplaceSystem.Web.UI.Interfaces;
using MarketplaceSystem.Web.UI.Interfaces.IBaseServices;
using MarketplaceSystem.Web.UI.Models.ApiResponses;
using MarketplaceSystem.Web.UI.Models.ApiResponses.Common;
using MarketplaceSystem.Web.UI.Models.ViewModels.Pages;
using MarketplaceSystem.Web.UI.Models.ViewModels.Product;

namespace MarketplaceSystem.Web.UI.Services
{
    public class ProductService : IProductService
    {
        private readonly IBaseApiService _baseApiService;
        public ProductService(IBaseApiService baseApiService)
        {
            _baseApiService = baseApiService;
        }
        public async Task<ApiResponse<PaginatedResponse<ListProductViewModel>>?> GetProductsAsync(
            ProductFilterViewModel? productFilterViewModel = null,
            CancellationToken cancellation = default)
        {
            HttpResponseMessage? response = await _baseApiService.GetAsync(
                endpoint: "v1/products",
                request: productFilterViewModel,
                cancellation: cancellation
            );
            return await response.Content
                .ReadFromJsonAsync<ApiResponse<PaginatedResponse<ListProductViewModel>>>(cancellationToken: cancellation);
        }

        public async Task<ApiResponse<ProductDetailViewModel>?> GetProductByIdAysnc(int id, CancellationToken cancellation = default)
        {
            HttpResponseMessage? response = await _baseApiService.GetAsync<object>(
                endpoint: "v1/products/" + id,
                cancellation: cancellation,
                request: null
                );

            return await response.Content
                .ReadFromJsonAsync<ApiResponse<ProductDetailViewModel>>(cancellationToken: cancellation);
        }

        public async Task<ApiResponse<ProductDetailViewModel>?> CreateProductAsync(
            CreatePostRequest createPostRequest,
            CancellationToken cancellation = default)
        {
            try
            {
                createPostRequest.Post.AttributeValues = createPostRequest.Post.AttributeValues
                    .Where(attribute =>
                        !string.IsNullOrEmpty(attribute.SelectValues) ||
                        !string.IsNullOrEmpty(attribute.TextValue) ||
                        attribute.NumberValue != null ||
                        attribute.BooleanValue != null ||
                        attribute.DateValue != null)
                    .ToList();

                HttpResponseMessage? response = await _baseApiService.PostAsync(
                endpoint: "user/v1/products",
                request: createPostRequest.Post,
                cancellation: cancellation
                );

                if (!response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ApiResponse<ProductDetailViewModel>>(cancellation);
                }

                ApiResponse<ProductDetailViewModel>? apiResponse = await response.Content
                    .ReadFromJsonAsync<ApiResponse<ProductDetailViewModel>>(cancellationToken: cancellation);

                if (apiResponse?.Data?.Id == null)
                {
                    return apiResponse;
                }

                if (createPostRequest.ImageViewModel.Images != null && createPostRequest.ImageViewModel.Images.Any())
                {
                    var additionalFields = new Dictionary<string, string>
                    {
                        { "productId", apiResponse.Data.Id.ToString() },
                        { "isMainIndex", createPostRequest.ImageViewModel.IsMainIndex.ToString() }
                    };

                    var images = new List<(Stream, string)>();
                    foreach (var image in createPostRequest.ImageViewModel.Images)
                    {
                        images.Add((image.OpenReadStream(), image.FileName));
                    }

                    HttpResponseMessage? responseImage = await _baseApiService.PostFilesAsync(
                        endpoint: "user/v1/files/upload-product-images",
                        files: images,
                        additionalFields: additionalFields,
                        cancellation: cancellation
                        );

                    foreach (var image in images)
                    {
                        image.Item1?.Dispose();
                    }

                    return apiResponse;
                }

                return apiResponse;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ApiResponse<object>?> MarkAsSoldAsync(int productId, CancellationToken cancellation = default)
        {
            try
            {
                HttpResponseMessage? response = await _baseApiService.PatchAsync<object>(
                    endpoint: $"user/v1/products/{productId}/mark-as-sold",
                    request: null,
                    cancellation: cancellation
                );

                return await response.Content.ReadFromJsonAsync<ApiResponse<object>>(cancellationToken: cancellation);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ApiResponse<object>?> DeleteProductAsync(int productId, CancellationToken cancellation = default)
        {
            try
            {
                HttpResponseMessage? response = await _baseApiService.DeleteAsync(
                    endpoint: $"user/v1/products/{productId}",
                    cancellation: cancellation
                );

                return await response.Content.ReadFromJsonAsync<ApiResponse<object>>(cancellationToken: cancellation);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
