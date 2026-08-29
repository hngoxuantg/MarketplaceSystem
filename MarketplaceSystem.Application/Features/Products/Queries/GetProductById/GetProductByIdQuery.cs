using MarketplaceSystem.Application.Common.DTOs.Products;
using MediatR;

namespace MarketplaceSystem.Application.Features.Products.Queries.GetProductById
{
    public record GetProductByIdQuery(int Id) : IRequest<ProductDto>;
}
