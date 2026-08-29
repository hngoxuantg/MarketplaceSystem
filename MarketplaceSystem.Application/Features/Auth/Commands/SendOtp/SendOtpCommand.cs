using MediatR;

namespace MarketplaceSystem.Application.Features.Auth.Commands.SendOtp
{
    public record SendOtpCommand(SendOtpRequest Request) : IRequest<string>;
}
