using Microsoft.AspNetCore.Http;

namespace MarketplaceSystem.Application.Features.Products.Commands.UploadImages
{
    public class UploadImagesRequest
    {
        public int ProductId { get; set; }

        public List<IFormFile> Images { get; set; }

        public int IsMainIndex { get; set; }
    }
}
