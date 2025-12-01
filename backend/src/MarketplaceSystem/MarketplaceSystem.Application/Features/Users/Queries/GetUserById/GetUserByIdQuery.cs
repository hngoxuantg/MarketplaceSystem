using MarketplaceSystem.Application.Common.DTOs.Users;
using MediatR;

namespace MarketplaceSystem.Application.Features.Users.Queries.GetUserById
{
    public record GetUserByIdQuery(int Id) : IRequest<UserDto>;
}
