using MediatR;

namespace MarketplaceSystem.Application.Features.Users.Commands.UnlockUser
{
    public record UnlockUserCommand(int Id) : IRequest<bool>;
}
