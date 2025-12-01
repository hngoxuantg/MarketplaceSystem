using FluentValidation;

namespace MarketplaceSystem.Application.Features.Categories.Commands.UpdateCategoryAttribute
{
    public class UpdateCategoryAttributeRequestValidator : AbstractValidator<UpdateCategoryAttributeRequest>
    {
        public UpdateCategoryAttributeRequestValidator()
        {
            RuleFor(ca => ca.Name)
                .NotEmpty().WithMessage("Tên thuộc tính là bắt buộc!")
                .MaximumLength(100).WithMessage("Tên thuộc tính không được quá 100 ký tự!")
                .MinimumLength(3).WithMessage("Tên thuộc tính ít nhất 3 ký tự!");

            RuleFor(ca => ca.DisplayName)
                .NotEmpty().WithMessage("Tên hiển thị là bắt buộc!")
                .MaximumLength(100).WithMessage("Tên hiển thị không được quá 100 ký tự!")
                .MinimumLength(3).WithMessage("Tên hiển thị ít nhất 3 ký tự!");

            RuleFor(ca => ca.DisplayOrder)
                .GreaterThan(0).WithMessage("Thứ tự hiển thị phải lớn hơn 0!");

            RuleFor(ca => ca.Placeholder)
                .MaximumLength(200).WithMessage("Gợi ý nhập không được quá 200 ký tự!");
        }
    }
}
