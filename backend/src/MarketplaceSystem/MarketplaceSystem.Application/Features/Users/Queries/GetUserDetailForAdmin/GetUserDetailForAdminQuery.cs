using MarketplaceSystem.Application.Common.DTOs.Users;
using MediatR;

namespace MarketplaceSystem.Application.Features.Users.Queries.GetUserDetailForAdmin
{
    public record GetUserDetailForAdminQuery(int Id) : IRequest<AdminUserDto>;
}
