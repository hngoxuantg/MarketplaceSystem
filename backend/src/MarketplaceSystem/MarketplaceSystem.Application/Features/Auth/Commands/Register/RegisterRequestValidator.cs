using FluentValidation;

namespace MarketplaceSystem.Application.Features.Auth.Commands.Register
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(r => r.FullName)
                .NotEmpty().WithMessage("Họ và tên không được để trống!")
                .MaximumLength(100).WithMessage("Họ và tên không được quá 100 kí tự!");

            RuleFor(r => r.Email)
                .NotEmpty().WithMessage("Email không được để trống!")
                .EmailAddress().WithMessage("Email không hợp lệ!");

            RuleFor(r => r.DateOfBirth)
                .NotEmpty().WithMessage("Ngày sinh không được để trống!")
                .LessThan(DateTime.Now).WithMessage("Ngày sinh phải trước ngày hiện tại!")
                .Must(date =>
                {
                    var age = DateTime.Today.Year - date.Year;
                    if (date > DateTime.Today.AddYears(-age)) age--;
                    return age >= 13 && age <= 100;
                }).WithMessage("Tuổi phải lớn hơn 13 và nhỏ hơn 150!");

            RuleFor(r => r.Location)
                .NotEmpty().WithMessage("Tỉnh/Thành phố không được để trống!")
                .IsInEnum().WithMessage("Tỉnh/Thành phố không hợp lệ!");

            RuleFor(r => r.Gender)
                .NotEmpty().WithMessage("Giới tính không được để trống!")
                .IsInEnum().WithMessage("Giới tính không hợp lệ!");

            RuleFor(r => r.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống!")
                .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 kí tự!")
                .Matches(@"[A-Z]").WithMessage("Mật khẩu phải chứa ít nhất 1 chữ hoa!")
                .Matches(@"[^a-zA-Z0-9]").WithMessage("Mật khẩu phải chứa ít nhất 1 kí tự đặc biệt!");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Mật khẩu xác nhận không khớp!");
        }
    }
}
