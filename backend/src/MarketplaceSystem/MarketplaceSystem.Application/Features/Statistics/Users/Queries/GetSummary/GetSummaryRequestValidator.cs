using FluentValidation;

namespace MarketplaceSystem.Application.Features.Statistics.Users.Queries.GetSummary
{
    public class GetSummaryRequestValidator : AbstractValidator<GetSummaryRequest>
    {
        public GetSummaryRequestValidator()
        {
            RuleFor(x => x.From)
                .NotEmpty().WithMessage("Ngày bắt đầu là bắt buộc!")
                .LessThanOrEqualTo(x => x.To).WithMessage("Ngày bắt đầu phải nhỏ hơn ngày kết thúc!")
                .ChildRules(from =>
                {
                    from.RuleFor(f => f)
                        .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Ngày bắt đầu không được lớn hơn ngày hiện tại!");
                });

            RuleFor(x => x.To)
                .NotEmpty().WithMessage("Ngày kết thúc là bắt buộc!")
                .GreaterThanOrEqualTo(x => x.From).WithMessage("Ngày kết thúc phải lớn hơn ngày bắt đầu!")
                .ChildRules(to =>
                {
                    to.RuleFor(t => t)
                        .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Ngày kết thúc không được lớn hơn ngày hiện tại!");
                });
        }
    }
}
