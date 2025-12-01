using FluentValidation;

namespace MarketplaceSystem.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductAttributeValueRequestValidator : AbstractValidator<CreateProductAttributeValueRequest>
    {
        public CreateProductAttributeValueRequestValidator()
        {
            RuleFor(a => a.CategoryAttributeId)
                .NotEmpty().WithMessage("Mã thuộc tính không được để trống!");
        }
    }
}
