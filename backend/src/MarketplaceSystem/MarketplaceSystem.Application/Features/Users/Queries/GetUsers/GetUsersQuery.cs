using MarketplaceSystem.Application.Common.DTOs.Users;
using MarketplaceSystem.Common.Models.Pagination;
using MediatR;

namespace MarketplaceSystem.Application.Features.Users.Queries.GetUsers
{
    public record GetUsersQuery(GetUsersRequest Request) : IRequest<PaginatedResult<AdminUserDto>>;
}
