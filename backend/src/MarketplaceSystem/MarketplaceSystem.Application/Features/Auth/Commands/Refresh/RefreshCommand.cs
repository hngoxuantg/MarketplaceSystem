using MarketplaceSystem.Application.Common.DTOs.Auths;
using MediatR;

namespace MarketplaceSystem.Application.Features.Auth.Commands.Refresh
{
    public record RefreshCommand(string RefreshToken, string Role) : IRequest<AuthDto>;
}
