using FluentValidation;

namespace MarketplaceSystem.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateAttributeOptionRequestValidator : AbstractValidator<CreateAttributeOptionRequest>
    {
        public CreateAttributeOptionRequestValidator()
        {
            RuleFor(o => o.Value)
                .NotEmpty().WithMessage("Giá trị tùy chọn là bắt buộc!")
                .MaximumLength(100).WithMessage("Giá trị tùy chọn không được quá 100 kí tự!")
                .MinimumLength(1).WithMessage("Giá trị tùy chọn ít nhất 1 kí tự!");

            RuleFor(o => o.DisplayText)
                .NotEmpty().WithMessage("Văn bản hiển thị là bắt buộc!")
                .MaximumLength(100).WithMessage("Văn bản hiển thị không được quá 100 kí tự!")
                .MinimumLength(1).WithMessage("Văn bản hiển thị ít nhất 1 kí tự!");
        }
    }
}
