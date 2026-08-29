using MarketplaceSystem.Application.Common.DTOs.Categories;
using MediatR;

namespace MarketplaceSystem.Application.Features.Categories.Queries.GetHomeCategories
{
    public record GetHomeCategoriesQuery : IRequest<RootCategoryCardDto>;
}
