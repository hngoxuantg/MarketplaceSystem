using MarketplaceSystem.Application.Common.DTOs.Categories;
using MarketplaceSystem.Application.Features.Categories.Commands.CreateCategory;
using MarketplaceSystem.Application.Features.Categories.Commands.CreateCategoryAttribute;
using MarketplaceSystem.Application.Features.Categories.Commands.DeleteAttributeOption;
using MarketplaceSystem.Application.Features.Categories.Commands.DeleteCategory;
using MarketplaceSystem.Application.Features.Categories.Commands.DeleteCategoryAttribute;
using MarketplaceSystem.Application.Features.Categories.Commands.ToggleActiveStatus;
using MarketplaceSystem.Application.Features.Categories.Commands.UpdateAttributeOption;
using MarketplaceSystem.Application.Features.Categories.Commands.UpdateCategory;
using MarketplaceSystem.Application.Features.Categories.Commands.UpdateCategoryAttribute;
using MarketplaceSystem.Application.Features.Categories.Queries.GetCategories;
using MarketplaceSystem.Application.Features.Categories.Queries.GetCategoryById;
using MarketplaceSystem.Common.Models.Pagination;
using MarketplaceSystem.Common.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceSystem.API.Controllers.Admin.V1
{
    public class CategoriesController : BaseController
    {
        private readonly ISender _sender;

        public CategoriesController(ISender sender)
        {
            _sender = sender;
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategoryAsync(
            [FromBody] CreateCategoryRequest createCategoryRequest,
            CancellationToken cancellation = default)
        {
            var result = await _sender.Send(new CreateCategoryCommand(createCategoryRequest), cancellation);

            return Ok(new ApiResponse<CategoryDto>
            {
                Success = true,
                Message = "Tạo danh mục thành công!",
                Data = result
            });
        }

        [HttpPost("{id}/attributes")]
        public async Task<IActionResult> CreateCategoryAttributeAsync(
            int id,
            [FromBody] Application.Features.Categories.Commands.CreateCategoryAttribute.CreateCategoryAttributeRequest createCategoryAttributeRequest,
            CancellationToken cancellation = default)
        {
            var result = await _sender.Send(new CreateCategoryAttributeCommand(id, createCategoryAttributeRequest), cancellation);

            return Ok(new ApiResponse<CategoryAttributeDto>
            {
                Success = true,
                Message = "Tạo thuộc tính cho danh mục thành công!",
                Data = result
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoriesAsync([FromQuery] GetCategoriesRequest filterRequest, CancellationToken cancellation = default)
        {
            var result = await _sender.Send(new GetCategoriesQuery(filterRequest), cancellation);

            return Ok(new ApiResponse<PaginatedResult<CategoryDto>>
            {
                Success = true,
                Message = "Lấy danh mục thành công!",
                Data = result
            });
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryByIdAsync(
            [FromRoute] int id,
            CancellationToken cancellation = default)
        {
            var result = await _sender.Send(new GetCategoryByIdQuery(id), cancellation);

            return Ok(new ApiResponse<CategoryDto>
            {
                Success = true,
                Message = "Lấy danh mục thành công!",
                Data = result
            });
        }

        [AllowAnonymous]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategoryAsync(
            int id,
            [FromBody] UpdateCategoryRequest updateCategoryRequest,
            CancellationToken cancellation = default)
        {
            var result = await _sender.Send(new UpdateCategoryCommand(
                id,
                updateCategoryRequest), cancellation);

            return Ok(new ApiResponse<CategoryDto>
            {
                Success = true,
                Message = "Cập nhật danh mục thành công!",
                Data = result
            });
        }

        [HttpPatch("{id}/attributes/{attributeId}")]
        public async Task<IActionResult> UpdateCategoryAttributeAsync(
            int id,
            int attributeId,
            [FromBody] UpdateCategoryAttributeRequest updateCategoryAttributeRequest,
            CancellationToken cancellation = default)
        {
            var result = await _sender.Send(new UpdateCategoryAttributeCommand(
                id,
                attributeId,
                updateCategoryAttributeRequest), cancellation);

            return Ok(new ApiResponse<CategoryAttributeDto>
            {
                Success = true,
                Message = "Cập nhật thuộc tính cho danh mục thành công!",
                Data = result
            });
        }

        [HttpPatch("{id}/attributes/{attributeId}/options/{optionId}")]
        public async Task<IActionResult> UpdateAttributeOptionAsync(
            int id,
            int attributeId,
            int optionId,
            [FromBody] UpdateAttributeOptionRequest attributeOptionRequest,
            CancellationToken cancellation = default)
        {
            var result = await _sender.Send(new UpdateAttributeOptionCommand(
                id,
                attributeId,
                optionId,
                attributeOptionRequest), cancellation);

            return Ok(new ApiResponse<AttributeOptionDto>
            {
                Success = true,
                Message = "Cập nhật lựa chọn danh mục thành công!",
                Data = result
            });
        }

        [HttpDelete("{id}/attributes/{attributeId}")]
        public async Task<IActionResult> DeleteAttributeAsync(
            int id,
            int attributeId,
            CancellationToken cancellation = default)
        {
            await _sender.Send(new DeleteCategoryAttributeCommand(id, attributeId), cancellation);

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Xóa thuộc tính danh mục thành công!"
            });
        }

        [HttpDelete("{id}/attributes/{attributeId}/options/{optionId}")]
        public async Task<IActionResult> DeleteAttributeOptionAsync(
            int id,
            int attributeId,
            int optionId,
            CancellationToken cancellation = default)
        {
            await _sender.Send(new DeleteAttributeOptionCommand(id, attributeId, optionId), cancellation);

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Xóa lựa chọn danh mục thành công!"
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoryAsync(int id, CancellationToken cancellation = default)
        {
            await _sender.Send(new DeleteCategoryCommand(id), cancellation);

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Xóa danh mục thành công!"
            });
        }

        [HttpPatch("{id}/toggle-active")]
        public async Task<IActionResult> ToggleActiveAsync(int id, CancellationToken cancellation = default)
        {
            await _sender.Send(new ToggleActiveStatusCommand(id), cancellation);

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Chuyển đổi trạng thái danh mục thành công!"
            });
        }
    }
}
