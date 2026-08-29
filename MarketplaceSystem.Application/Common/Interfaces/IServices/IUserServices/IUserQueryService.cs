using MarketplaceSystem.Application.Common.DTOs.Products;
using MarketplaceSystem.Application.Features.Users.Queries.GetProducts;
using MarketplaceSystem.Common.Models.Pagination;

namespace MarketplaceSystem.Application.Common.Interfaces.IServices.IUserServices
{
    public interface IUserQueryService
    {
        Task<PaginatedResult<ProductCardDto>> GetProducts(int userId, UserProductsFilterQuery filter, CancellationToken cancellation = default);
    }
}