using FluentValidation;

namespace MarketplaceSystem.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductRequestValidator()
        {
            RuleFor(p => p.Title)
                .NotEmpty().WithMessage("Tiêu đề không được để trống.")
                .MaximumLength(100).WithMessage("Tiêu đề không được vượt quá 100 ký tự.");

            RuleFor(p => p.Description)
                .MaximumLength(300).WithMessage("Mô tả không được vượt quá 300 ký tự.");

            RuleFor(p => p.Price)
                .NotEmpty().WithMessage("Giá không được để trống.");

            RuleFor(p => p.Quantity)
                .GreaterThan(0).WithMessage("Số lượng phải lớn hơn 0.");

            RuleFor(p => p.Condition)
                .IsInEnum().WithMessage("Tình trạng sản phẩm không hợp lệ.");

            RuleFor(p => p.CategoryId)
                .NotEmpty().WithMessage("Danh mục không được để trống.");

            RuleFor(p => p.Location)
                .IsInEnum().WithMessage("Vị trí không hợp lệ.");

            RuleFor(p => p.AttributeValues)
                .NotEmpty().WithMessage("Thuộc tính sản phẩm không được để trống.");

            RuleForEach(p => p.AttributeValues)
                .SetValidator(new CreateProductAttributeValueRequestValidator())
                .When(p => p.AttributeValues != null && p.AttributeValues.Any());
        }
    }
}
