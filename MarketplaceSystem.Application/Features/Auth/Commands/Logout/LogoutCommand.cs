using MediatR;

namespace MarketplaceSystem.Application.Features.Auth.Commands.Logout
{
    public record LogoutCommand(string RefreshToken) : IRequest<bool>;
}
