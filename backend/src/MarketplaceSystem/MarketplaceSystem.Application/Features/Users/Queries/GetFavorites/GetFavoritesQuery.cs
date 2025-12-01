using MarketplaceSystem.Application.Common.DTOs.Products;
using MarketplaceSystem.Common.Models.Pagination;
using MediatR;

namespace MarketplaceSystem.Application.Features.Users.Queries.GetFavorites
{
    public record GetFavoritesQuery(int UserId, GetFavoritesRequest Request) : IRequest<PaginatedResult<ProductCardDto>>;
}
