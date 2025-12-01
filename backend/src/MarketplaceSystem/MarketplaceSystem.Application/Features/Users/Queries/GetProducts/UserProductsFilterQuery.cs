using MarketplaceSystem.Application.Common.DTOs.Products;
using MarketplaceSystem.Common.Models.Pagination;
using MediatR;

namespace MarketplaceSystem.Application.Features.Users.Queries.GetProducts
{
    public record UserProductsFilterQuery(int UserId, UserProductsFilterRequest Request) : IRequest<PaginatedResult<ProductCardDto>>;
}
