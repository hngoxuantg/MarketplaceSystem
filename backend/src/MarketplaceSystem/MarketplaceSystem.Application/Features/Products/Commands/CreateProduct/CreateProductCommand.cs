using MarketplaceSystem.Application.Common.DTOs.Products;
using MediatR;

namespace MarketplaceSystem.Application.Features.Products.Commands.CreateProduct
{
    public record CreateProductCommand(CreateProductRequest Request) : IRequest<ProductDto>;
}
