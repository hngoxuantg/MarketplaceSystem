using MarketplaceSystem.Application.Common.DTOs.Products;
using MarketplaceSystem.Application.Features.Products.Commands.UploadImages;
using MarketplaceSystem.Common.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceSystem.API.Controllers.User.V1
{
    public class FilesController : BaseController
    {
        private readonly ISender _sender;
        public FilesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("upload-product-images")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadProductImagesAsync([FromForm] UploadImagesRequest imagesRequest, CancellationToken cancellation = default)
        {
            var result = await _sender.Send(new UploadImagesCommand(imagesRequest), cancellation);

            var response = new ApiResponse<ProductImagesDto>
            {
                Success = true,
                Message = "Upload ảnh cho sản phẩm thành công!",
                Data = result
            };

            return Ok(response);
        }
    }
}
