using FluentValidation;

namespace MarketplaceSystem.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
    {
        public CreateCategoryRequestValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Tên danh mục là bắt buộc!")
                .MaximumLength(100).WithMessage("Tên danh mục không được quá 100 kí tự!")
                .MinimumLength(3).WithMessage("Tên danh mục ít nhất 3 kí tự!");

            RuleFor(c => c.Description)
                .MaximumLength(100).WithMessage("Mô tả không được quá 100 kí tự!");

            RuleFor(c => c.IsActive)
                .NotEmpty().WithMessage("Trạng thái kích hoạt là bắt buộc!");

            RuleForEach(c => c.Attributes)
                .SetValidator(new CreateCategoryAttributeRequestValidator())
                .When(c => c.Attributes != null && c.Attributes.Count > 0);

            RuleFor(c => c.DisplayOrder)
                .GreaterThan(0).WithMessage("Thứ tự hiển thị phải lớn hơn 0!");

            RuleFor(c => c.Attributes)
                .NotEmpty().WithMessage("Danh mục con phải có ít nhất một thuộc tính!")
                .When(c => c.ParentCategoryId.HasValue);

            RuleFor(c => c.Attributes)
                .Empty().WithMessage("Danh mục cha không được có thuộc tính!")
                .When(c => !c.ParentCategoryId.HasValue);

            RuleFor(c => c.ParentCategoryId);
        }
    }
}
