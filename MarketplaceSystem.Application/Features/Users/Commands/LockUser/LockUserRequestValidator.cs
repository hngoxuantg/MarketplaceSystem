using FluentValidation;

namespace MarketplaceSystem.Application.Features.Users.Commands.LockUser
{
    public class LockUserRequestValidator : AbstractValidator<LockUserRequest>
    {
        public LockUserRequestValidator()
        {
            RuleFor(r => r.Until)
                .NotEmpty().WithMessage("Thời gian khóa không được để trống!")
                .GreaterThan(DateTime.Now).WithMessage("Thời gian khóa phải sau thời gian hiện tại!");
        }
    }
}
