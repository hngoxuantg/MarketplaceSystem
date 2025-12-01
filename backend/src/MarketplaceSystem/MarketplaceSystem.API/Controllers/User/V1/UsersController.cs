using MarketplaceSystem.Application.Common.DTOs.Products;
using MarketplaceSystem.Application.Common.DTOs.Users;
using MarketplaceSystem.Application.Features.Users.Commands.AddFavorite;
using MarketplaceSystem.Application.Features.Users.Commands.RemoveFavorite;
using MarketplaceSystem.Application.Features.Users.Queries.GetFavorites;
using MarketplaceSystem.Application.Features.Users.Queries.GetProducts;
using MarketplaceSystem.Application.Features.Users.Queries.GetUserById;
using MarketplaceSystem.Common.Models.Pagination;
using MarketplaceSystem.Common.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceSystem.API.Controllers.User.V1
{
    public class UsersController : BaseController
    {
        private readonly ISender _sender;
        public UsersController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id, CancellationToken cancellation = default)
        {
            var result = await _sender.Send(new GetUserByIdQuery(id), cancellation);

            var response = new ApiResponse<UserDto>
            {
                Success = true,
                Message = "Lấy thông tin người dùng thành công",
                Data = result
            };

            return Ok(response);
        }

        [HttpPost("{userId}/favorites")]
        [AllowAnonymous]
        public async Task<IActionResult> AddFavoriteAsync(int userId, [FromBody] int productId, CancellationToken cancellation = default)
        {
            bool result = await _sender.Send(new AddFavoriteCommand(userId, productId), cancellation);

            if (!result)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Sản phẩm đã có trong danh sách yêu thích"
                });
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Thêm sản phẩm yêu thích thành công"
            });
        }

        [HttpDelete("{userId}/favorites/{productId}")]
        public async Task<IActionResult> RemoveFavoriteAsync(int userId, int productId, CancellationToken cancellation = default)
        {
            bool result = await _sender.Send(new RemoveFavoriteCommand(userId, productId), cancellation);

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Xóa sản phẩm yêu thích thành công"
            });
        }

        [HttpGet("{userId}/favorites")]
        public async Task<IActionResult> GetFavoritesAsync(int userId, [FromQuery] GetFavoritesRequest getFavoritesRequest, CancellationToken cancellation = default)
        {
            var result = await _sender.Send(new GetFavoritesQuery(userId, getFavoritesRequest), cancellation);

            return Ok(new ApiResponse<PaginatedResult<ProductCardDto>>
            {
                Success = true,
                Message = "Lấy danh sách sản phẩm yêu thích thành công",
                Data = result
            });
        }

        [HttpGet("{userId}/products")]
        public async Task<IActionResult> GetProductsAsync(
            int userId,
            [FromQuery] UserProductsFilterRequest userProductsFilterQuery,
            CancellationToken cancellation = default)
        {
            var result = await _sender.Send(new UserProductsFilterQuery(userId, userProductsFilterQuery), cancellation);

            return Ok(new ApiResponse<PaginatedResult<ProductCardDto>>
            {
                Success = true,
                Message = "Lấy danh sách sản phẩm của người dùng thành công",
                Data = result
            });
        }
    }
}