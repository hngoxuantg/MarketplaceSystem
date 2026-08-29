using FluentValidation;

namespace MarketplaceSystem.Application.Features.Auth.Commands.VerifyOtp
{
    public class VerifyOtpRequestValidator : AbstractValidator<VerifyOtpRequest>
    {
        public VerifyOtpRequestValidator()
        {
            RuleFor(v => v.Email)
                .NotEmpty().WithMessage("Email không được để trống!")
                .EmailAddress().WithMessage("Email không hợp lệ!");

            RuleFor(v => v.Otp)
                .NotEmpty().WithMessage("Mã OTP không được để trống!")
                .Length(6).WithMessage("Mã OTP phải có đúng 6 kí tự!");
        }
    }
}
