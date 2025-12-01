using MarketplaceSystem.Application.Common.DTOs.Categories;
using MediatR;

namespace MarketplaceSystem.Application.Features.Categories.Queries.GetListingCategories
{
    public record GetListingCategoriesQuery : IRequest<List<ListingCategoryDto>>;
}
