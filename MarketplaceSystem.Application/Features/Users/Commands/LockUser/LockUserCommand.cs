using MediatR;

namespace MarketplaceSystem.Application.Features.Users.Commands.LockUser
{
    public record LockUserCommand(int Id, LockUserRequest Request) : IRequest<bool>;
}
