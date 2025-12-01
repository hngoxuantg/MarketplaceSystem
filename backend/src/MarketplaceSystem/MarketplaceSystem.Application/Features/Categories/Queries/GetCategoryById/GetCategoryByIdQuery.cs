using MarketplaceSystem.Application.Common.DTOs.Categories;
using MediatR;

namespace MarketplaceSystem.Application.Features.Categories.Queries.GetCategoryById
{
    public record GetCategoryByIdQuery(int Id) : IRequest<CategoryDto>;
}
