using MarketplaceSystem.Application.Common.DTOs.Categories;
using MarketplaceSystem.Application.Features.Categories.Queries.GetAttributesByCategoryId;
using MarketplaceSystem.Application.Features.Categories.Queries.GetListingCategories;
using MarketplaceSystem.Common.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace MarketplaceSystem.API.Controllers.User.V1
{
    public class CategoriesController : BaseController
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ISender _sender;

        public CategoriesController(ISender sender, IMemoryCache memoryCache)
        {
            _sender = sender;
            _memoryCache = memoryCache;
        }


        [HttpGet("listing-categories")]
        public async Task<IActionResult> GetListingCategoriesAsync(CancellationToken cancellation = default)
        {
            var result = await _sender.Send(new GetListingCategoriesQuery(), cancellation);

            var response = new ApiResponse<List<ListingCategoryDto>>()
            {
                Success = true,
                Message = "Lấy danh mục thành công!",
                Data = result
            };

            return Ok(response);
        }

        [HttpGet("{id}/attributes")]
        public async Task<IActionResult> GetAttributesAsync(int id, CancellationToken cancellation = default)
        {
            var result = await _sender.Send(new GetAttributesByCategoryIdQuery(id), cancellation);

            return Ok(new ApiResponse<List<CategoryAttributeDto>>
            {
                Success = true,
                Message = "Lấy thuộc tính cho danh mục thành công!",
                Data = result
            });
        }
    }
}
