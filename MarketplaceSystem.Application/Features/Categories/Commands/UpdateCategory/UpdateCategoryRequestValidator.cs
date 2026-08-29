using FluentValidation;

namespace MarketplaceSystem.Application.Features.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
    {
        public UpdateCategoryRequestValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Tên danh mục không được để trống.")
                .MaximumLength(100).WithMessage("Tên danh mục không được vượt quá 100 ký tự.");

            RuleFor(c => c.Description)
                .MaximumLength(500).WithMessage("Mô tả danh mục không được vượt quá 500 ký tự.");

            RuleFor(c => c.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Thứ tự hiển thị phải là số nguyên không âm.");

            RuleFor(c => c.IsActive)
                .NotNull().WithMessage("Trạng thái kích hoạt không được để trống.");
        }
    }
}
