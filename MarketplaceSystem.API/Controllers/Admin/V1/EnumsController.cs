using MarketplaceSystem.Application.Features.Products.Queries.GetProductsByFilterAdmin;
using MarketplaceSystem.Common.Helpers;
using MarketplaceSystem.Common.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceSystem.API.Controllers.Admin.V1
{
    public class EnumsController : BaseController
    {
        [HttpGet("sort-options")]
        public IActionResult GetSortOptions()
        {
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Lấy danh sách tùy chọn sắp xếp thành công!",
                Data = EnumHelper.ToList<ProductAdminSortBy>(),
            });
        }
    }
}
