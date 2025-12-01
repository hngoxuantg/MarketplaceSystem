using MarketplaceSystem.Application.Common.DTOs.Products;
using MarketplaceSystem.Common.Models.Pagination;
using MediatR;

namespace MarketplaceSystem.Application.Features.Products.Queries.GetProductsByFilter
{
    public record ProductsByFilterQuery(ProductsByFilterRequest Request) : IRequest<PaginatedResult<ProductCardDto>>;
}
