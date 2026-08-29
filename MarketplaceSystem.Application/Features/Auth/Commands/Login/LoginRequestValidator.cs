using FluentValidation;

namespace MarketplaceSystem.Application.Features.Auth.Commands.Login
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(l => l.Email)
                .NotEmpty().WithMessage($"{nameof(LoginRequest.Email)} là bắt buộc!")
                .EmailAddress().WithMessage("Email không đúng định dạng!");

            RuleFor(l => l.Password)
                .NotEmpty().WithMessage($"{nameof(LoginRequest.Password)} là bắt buộc")
                .MinimumLength(6).WithMessage($"{nameof(LoginRequest.Password)} ít nhất 6 kí tự")
                .MaximumLength(50).WithMessage("Không được quá 50 kí tự!");
        }
    }
}
