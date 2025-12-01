using MarketplaceSystem.Application.Common.DTOs.Products;
using MarketplaceSystem.Common.Models.Pagination;
using MediatR;

namespace MarketplaceSystem.Application.Features.Products.Queries.GetProductsByFilterAdmin
{
    public record GetProductsByFilterAdminQuery(GetProductsByFilterAdminRequest Request) : IRequest<PaginatedResult<ProductAdminCardDto>>;
}
