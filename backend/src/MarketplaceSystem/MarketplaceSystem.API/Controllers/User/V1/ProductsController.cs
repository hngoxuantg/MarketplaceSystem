using MarketplaceSystem.Application.Common.DTOs.Products;
using MarketplaceSystem.Application.Features.Products.Commands.CreateProduct;
using MarketplaceSystem.Application.Features.Products.Commands.DeleteProduct;
using MarketplaceSystem.Application.Features.Products.Commands.MarkAsSold;
using MarketplaceSystem.Application.Features.Products.Queries.GetMyProductById;
using MarketplaceSystem.Common.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace MarketplaceSystem.API.Controllers.User.V1
{
    public class ProductsController : BaseController
    {
        private readonly ISender _sender;
        private readonly IMemoryCache _memoryCache;
        public ProductsController(ISender sender, IMemoryCache memoryCache)
        {
            _sender = sender;
            _memoryCache = memoryCache;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateProductAsync([FromBody] CreateProductRequest createProductRequest, CancellationToken cancellation = default)
        {
            var result = await _sender.Send(new CreateProductCommand(createProductRequest), cancellation);

            return Ok(new ApiResponse<ProductDto>
            {
                Success = true,
                Message = "Tạo sản phẩm thành công!",
                Data = result
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductAsync(int id, CancellationToken cancellation = default)
        {
            var result = await _sender.Send(new DeleteProductCommand(id), cancellation);

            _memoryCache.Remove($"Product_{id}");

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Gỡ sản phẩm thành công!"
            });
        }

        [HttpPut("{id}/mark-as-sold")]
        public async Task<IActionResult> MarkAsSoldAsync(int id, CancellationToken cancellation = default)
        {
            var result = await _sender.Send(new MarkAsSoldCommand(id), cancellation);

            _memoryCache.Remove($"Product_{id}");

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Đánh dấu sản phẩm đã bán thành công!"
            });
        }

        [HttpGet("my-products/{id}")]
        public async Task<IActionResult> GetMyProductByIdAsync(int id, CancellationToken cancellation = default)
        {
            var result = await _sender.Send(new GetMyProductByIdQuery(id), cancellation);

            return Ok(new ApiResponse<ProductDto>
            {
                Success = true,
                Message = "Lấy thông tin sản phẩm thành công!",
                Data = result
            });
        }
    }
}
