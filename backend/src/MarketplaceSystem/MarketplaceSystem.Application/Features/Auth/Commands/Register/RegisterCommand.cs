using MediatR;

namespace MarketplaceSystem.Application.Features.Auth.Commands.Register
{
    public record RegisterCommand(RegisterRequest Request) : IRequest<Unit>;
}