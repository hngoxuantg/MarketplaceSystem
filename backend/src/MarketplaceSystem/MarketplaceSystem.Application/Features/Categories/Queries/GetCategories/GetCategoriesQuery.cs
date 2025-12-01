using MarketplaceSystem.Application.Common.DTOs.Categories;
using MarketplaceSystem.Common.Models.Pagination;
using MediatR;

namespace MarketplaceSystem.Application.Features.Categories.Queries.GetCategories
{
    public record GetCategoriesQuery(GetCategoriesRequest Request) : IRequest<PaginatedResult<CategoryDto>>;
}
