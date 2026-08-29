using MarketplaceSystem.Application.Common.DTOs.Categories;
using MarketplaceSystem.Application.Features.Categories.Queries.GetHomeCategories;
using MarketplaceSystem.Common.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace MarketplaceSystem.API.Controllers.Public.V1
{
    public class CategoriesController : BaseController
    {
        private readonly ISender _sender;
        private readonly IMemoryCache _memoryCache;

        public CategoriesController(ISender sender, IMemoryCache memoryCache)
        {
            _sender = sender;
            _memoryCache = memoryCache;
        }

        [HttpGet("home")]
        public async Task<IActionResult> GetHomeCategories(CancellationToken cancellation = default)
        {
            RootCategoryCardDto result;
            if (_memoryCache.TryGetValue("home_categories", out RootCategoryCardDto? cachedCategories))
            {
                result = cachedCategories;
            }
            else
            {
                result = await _sender.Send(new GetHomeCategoriesQuery(), cancellation);
                if (result != null)
                    _memoryCache.Set("home_categories", result, TimeSpan.FromMinutes(10));
            }

            var response = new ApiResponse<RootCategoryCardDto>()
            {
                Success = true,
                Message = "Lấy danh mục thành công!",
                Data = result
            };

            return Ok(response);
        }
    }
}
