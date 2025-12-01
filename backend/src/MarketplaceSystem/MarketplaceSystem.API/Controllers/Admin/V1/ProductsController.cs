using MarketplaceSystem.Application.Common.DTOs.Products;
using MarketplaceSystem.Application.Features.Products.Commands.ApproveProduct;
using MarketplaceSystem.Application.Features.Products.Commands.DeleteProductByAdmin;
using MarketplaceSystem.Application.Features.Products.Commands.RejectProduct;
using MarketplaceSystem.Application.Features.Products.Queries.GetProductByIdAdmin;
using MarketplaceSystem.Application.Features.Products.Queries.GetProductsByFilterAdmin;
using MarketplaceSystem.Common.Models.Pagination;
using MarketplaceSystem.Common.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceSystem.API.Controllers.Admin.V1
{
    public class ProductsController : BaseController
    {
        private readonly ISender _sender;

        public ProductsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsAsync([FromQuery] GetProductsByFilterAdminRequest filterQuery, CancellationToken cancellation = default)
        {
            var result = await _sender.Send(new GetProductsByFilterAdminQuery(filterQuery), cancellation);

            return Ok(new ApiResponse<PaginatedResult<ProductAdminCardDto>>
            {
                Success = true,
                Message = "Lấy danh sách sản phẩm thành công!",
                Data = result
            });
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetProductByIdAsync(int id, CancellationToken cancellation = default)
        {
            var result = _sender.Send(new GetProductByIdAdminQuery(id), cancellation);

            return Ok(new ApiResponse<ProductAdminCardDto>
            {
                Success = true,
                Message = "Lấy thông tin sản phẩm thành công!",
                Data = result.Result
            });
        }

        [HttpPost("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApprovedAsync(int id, CancellationToken cancellation = default)
        {
            await _sender.Send(new ApproveProductCommand(id), cancellation);

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Duyệt sản phẩm thành công!"
            });
        }

        [HttpPost("{id}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectedAsync(int id, RejectProductRequest rejectProductRequest, CancellationToken cancellation = default)
        {
            await _sender.Send(new RejectProductCommand(id, rejectProductRequest), cancellation);

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Từ chối sản phẩm thành công!"
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductAsync(int id, CancellationToken cancellation = default)
        {
            await _sender.Send(new DeleteProductByAdminCommand(id), cancellation);

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Xóa sản phẩm thành công!"
            });
        }
    }
}
