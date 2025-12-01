using FluentValidation;

namespace MarketplaceSystem.Application.Features.Statistics.Products.Queries.GetSummary
{
    public class GetProductSummaryRequestValidator : AbstractValidator<GetProductSummaryRequest>
    {
        public GetProductSummaryRequestValidator()
        {
            RuleFor(x => x.From)
                .NotEmpty()
                .WithMessage("Ngày bắt đầu không được để trống");

            RuleFor(x => x.To)
                .NotEmpty()
                .WithMessage("Ngày kết thúc không được để trống")
                .GreaterThanOrEqualTo(x => x.From)
                .WithMessage("Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu");
        }
    }
}
