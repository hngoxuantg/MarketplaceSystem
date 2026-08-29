using MarketplaceSystem.Application.Common.DTOs.Statistics.Users;
using MediatR;

namespace MarketplaceSystem.Application.Features.Statistics.Users.Queries.GetSummary
{
    public record GetSummaryQuery(GetSummaryRequest Request) : IRequest<SummaryDto>;
}
