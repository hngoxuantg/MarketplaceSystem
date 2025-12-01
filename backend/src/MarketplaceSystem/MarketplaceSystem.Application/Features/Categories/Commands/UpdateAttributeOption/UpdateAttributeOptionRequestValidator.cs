using FluentValidation;

namespace MarketplaceSystem.Application.Features.Categories.Commands.UpdateAttributeOption
{
    public class UpdateAttributeOptionRequestValidator : AbstractValidator<UpdateAttributeOptionRequest>
    {
        public UpdateAttributeOptionRequestValidator()
        {
            RuleFor(o => o.Value)
                .NotEmpty().WithMessage("Giá trị tùy chọn là bắt buộc!")
                .MaximumLength(100).WithMessage("Giá trị tùy chọn không được quá 100 ký tự!")
                .MinimumLength(1).WithMessage("Giá trị tùy chọn ít nhất 1 ký tự!");

            RuleFor(o => o.DisplayText)
                .NotEmpty().WithMessage("Văn bản hiển thị là bắt buộc!")
                .MaximumLength(100).WithMessage("Văn bản hiển thị không được quá 100 ký tự!")
                .MinimumLength(1).WithMessage("Văn bản hiển thị ít nhất 1 ký tự!");
        }
    }
}
