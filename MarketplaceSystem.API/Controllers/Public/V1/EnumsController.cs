using MarketplaceSystem.Common.Enums;
using MarketplaceSystem.Common.Helpers;
using MarketplaceSystem.Common.Models.Responses;
using MarketplaceSystem.Domain.Enums.Business;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceSystem.API.Controllers.Public.V1
{
    public class EnumsController : BaseController
    {
        [HttpGet("vietnam-provinces")]
        public IActionResult GetProvinces()
        {
            var result = EnumHelper.ToList<VietnamProvince>();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Lấy danh sách tỉnh thành phố thành công!",
                Data = result
            });
        }

        [HttpGet("product-conditions")]
        public IActionResult GetProductConditions()
        {
            var result = EnumHelper.ToList<ProductCondition>();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Lấy danh sách tình trạng sản phẩm thành công!",
                Data = result
            });
        }

        [HttpGet("category-attribute-types")]
        public IActionResult GetCategoryAttributeTypes()
        {
            var result = EnumHelper.ToList<AttributeType>();
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Lấy danh sách thuộc tính danh mục thành công!",
                Data = result
            });
        }
    }
}
