using MarketplaceSystem.Application.Common.DTOs.Products;
using MediatR;

namespace MarketplaceSystem.Application.Features.Products.Queries.GetMyProductById
{
    public record GetMyProductByIdQuery(int ProductId) : IRequest<ProductDto>;
}
