using MarketplaceSystem.Application.Common.DTOs.Products;
using MarketplaceSystem.Application.Features.Products.Commands.IncrementViewCount;
using MarketplaceSystem.Application.Features.Products.Queries.GetProductById;
using MarketplaceSystem.Application.Features.Products.Queries.GetProductsByFilter;
using MarketplaceSystem.Common.Models.Pagination;
using MarketplaceSystem.Common.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace MarketplaceSystem.API.Controllers.Public.V1
{
    public class ProductsController : BaseController
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ISender _sender;
        public ProductsController(IMemoryCache memoryCache, ISender sender)
        {
            _memoryCache = memoryCache;
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsByFilter([FromQuery] ProductsByFilterRequest productFilterRequest, CancellationToken cancellation = default)
        {
            var result = await _sender.Send(new ProductsByFilterQuery(productFilterRequest), cancellation);

            var response = new ApiResponse<PaginatedResult<ProductCardDto>>()
            {
                Success = true,
                Message = "Lấy sản phẩm thành công!",
                Data = result
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductByIdAysnc(int id, CancellationToken cancellation = default)
        {
            ProductDto? result;

            if (_memoryCache.TryGetValue($"Product_{id}", out ProductDto? cacheProduct))
            {
                result = cacheProduct;
            }
            else
            {
                result = await _sender.Send(new GetProductByIdQuery(id), cancellation);
                await _sender.Send(new IncrementViewCountCommand(id), cancellation);

                if (result != null)
                {
                    _memoryCache.Set($"Product_{id}", result, TimeSpan.FromMinutes(10));
                }
            }

            var response = new ApiResponse<ProductDto>
            {
                Success = true,
                Message = "Lấy sản phẩm thành công!",
                Data = result
            };

            return Ok(response);
        }
    }
}
