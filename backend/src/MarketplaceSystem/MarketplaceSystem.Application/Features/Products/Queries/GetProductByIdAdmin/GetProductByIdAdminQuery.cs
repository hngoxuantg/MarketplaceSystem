using MarketplaceSystem.Application.Common.DTOs.Products;
using MediatR;

namespace MarketplaceSystem.Application.Features.Products.Queries.GetProductByIdAdmin
{
    public record GetProductByIdAdminQuery(int Id) : IRequest<ProductAdminCardDto>;
}
