using FluentValidation;

namespace MarketplaceSystem.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryAttributeRequestValidator : AbstractValidator<CreateCategoryAttributeRequest>
    {
        public CreateCategoryAttributeRequestValidator()
        {
            RuleFor(ca => ca.Name)
                .NotEmpty().WithMessage("Tên thuộc tính là bắt buộc!")
                .MaximumLength(100).WithMessage("Tên thuộc tính không được quá 100 kí tự!")
                .MinimumLength(3).WithMessage("Tên thuộc tính ít nhất 3 kí tự!");

            RuleFor(ca => ca.DisplayName)
                .NotEmpty().WithMessage("Tên hiển thị là bắt buộc!")
                .MaximumLength(100).WithMessage("Tên hiển thị không được quá 100 kí tự!")
                .MinimumLength(3).WithMessage("Tên hiển thị ít nhất 3 kí tự!");

            RuleFor(ca => ca.AttributeType)
                .IsInEnum().WithMessage("Loại thuộc tính không hợp lệ!");

            RuleFor(ca => ca.DisplayOrder)
                .NotEmpty().WithMessage("Thứ tự hiển thị là bắt buộc!")
                .GreaterThan(0).WithMessage("Thứ tự hiển thị phải lớn hơn 0!");

            RuleFor(ca => ca.Placeholder)
                .MaximumLength(200).WithMessage("Gợi ý nhập không được quá 200 kí tự!");

            RuleFor(ca => ca.AttributeOptions)
                .NotEmpty().WithMessage("Thuộc tính phải có ít nhất một tùy chọn!")
                .When(ca => ca.AttributeType == Domain.Enums.Business.AttributeType.Select);
        }
    }
}
