using MarketplaceSystem.Application.Common.DTOs.Auths;
using MediatR;

namespace MarketplaceSystem.Application.Features.Auth.Commands.Login
{
    public record LoginCommand(string Role, LoginRequest Request) : IRequest<AuthDto>;
}
