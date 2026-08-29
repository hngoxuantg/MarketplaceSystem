using MediatR;

namespace MarketplaceSystem.Application.Features.Auth.Commands.VerifyOtp
{
    public record VerifyOtpCommand(VerifyOtpRequest Request) : IRequest<bool>;
}
