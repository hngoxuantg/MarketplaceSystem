using MarketplaceSystem.Application.Common.DTOs.Statistics.Products;
using MediatR;

namespace MarketplaceSystem.Application.Features.Statistics.Products.Queries.GetSummary
{
    public class GetProductSummaryQuery : IRequest<ProductSummaryDto>
    {
        public GetProductSummaryRequest Request { get; set; }

        public GetProductSummaryQuery(GetProductSummaryRequest request)
        {
            Request = request;
        }
    }
}
