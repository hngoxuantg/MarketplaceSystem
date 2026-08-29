using MarketplaceSystem.Application.Common.DTOs.Users;
using MarketplaceSystem.Application.Features.Users.Commands.LockUser;
using MarketplaceSystem.Application.Features.Users.Commands.UnlockUser;
using MarketplaceSystem.Application.Features.Users.Queries.GetUserDetailForAdmin;
using MarketplaceSystem.Application.Features.Users.Queries.GetUsers;
using MarketplaceSystem.Common.Models.Pagination;
using MarketplaceSystem.Common.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceSystem.API.Controllers.Admin.V1
{
    public class UsersController : BaseController
    {
        private readonly ISender _sender;

        public UsersController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersAsync(
            [FromQuery] GetUsersRequest getUsersRequest,
            CancellationToken cancellation = default)
        {
            var result = await _sender.Send(new GetUsersQuery(getUsersRequest), cancellation);
            return Ok(new ApiResponse<PaginatedResult<AdminUserDto>>
            {
                Success = true,
                Message = "Lấy danh sách người dùng thành công!",
                Data = result
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUserByIdAsync(int id, CancellationToken cancellation = default)
        {
            var result = await _sender.Send(new GetUserDetailForAdminQuery(id), cancellation);

            return Ok(new ApiResponse<AdminUserDto>
            {
                Success = true,
                Message = "Lấy thông tin người dùng thành công!",
                Data = result
            });
        }

        [HttpPost("{id}/lock")]
        public async Task<IActionResult> LockUserAsync(int id, LockUserRequest request, CancellationToken cancellation = default)
        {
            var result = await _sender.Send(new LockUserCommand(id, request));

            return Ok(new ApiResponse
            {
                Success = result,
                Message = result ? "Khóa người dùng thành công!" : "Khóa người dùng thất bại!",
            });
        }

        [HttpPost("{id}/unlock")]
        public async Task<IActionResult> UnlockUserAsync(int id, CancellationToken cancellation = default)
        {
            var result = await _sender.Send(new UnlockUserCommand(id));

            return Ok(new ApiResponse
            {
                Success = result,
                Message = result ? "Mở khóa người dùng thành công!" : "Mở khóa người dùng thất bại!",
            });
        }
    }
}
