using FluentValidation;

namespace MarketplaceSystem.Application.Features.Auth.Commands.SendOtp
{
    public class SendOtpRequestValidator : AbstractValidator<SendOtpRequest>
    {
        public SendOtpRequestValidator()
        {
            RuleFor(s => s.Email)
                .NotEmpty().WithMessage("Email không được để trống!")
                .EmailAddress().WithMessage("Email không hợp lệ!");
        }
    }
}
