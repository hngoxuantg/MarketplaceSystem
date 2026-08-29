using FluentValidation;

namespace MarketplaceSystem.Application.Features.Products.Commands.UploadImages
{
    public class UploadImagesRequestValidator : AbstractValidator<UploadImagesRequest>
    {
        public UploadImagesRequestValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("ProductId không được để trống.")
                .GreaterThan(0).WithMessage("ProductId phải lớn hơn 0.");

            RuleFor(x => x.Images)
                .NotEmpty().WithMessage("Danh sách ảnh không được để trống.")
                .Must(images => images != null && images.Count > 0).WithMessage("Phải có ít nhất một ảnh được tải lên.")
                .Must(images => images != null && images.Count <= 10).WithMessage("Không được tải lên quá 10 ảnh.");

            RuleFor(x => x.IsMainIndex)
                .GreaterThanOrEqualTo(0).WithMessage("Chỉ số ảnh chính phải lớn hơn hoặc bằng 0.")
                .Must((dto, isMainIndex) => dto.Images != null && isMainIndex < dto.Images.Count)
                .WithMessage("Chỉ số ảnh chính phải nhỏ hơn số lượng ảnh được tải lên.");
        }
    }
}
