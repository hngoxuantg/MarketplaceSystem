using MarketplaceSystem.Application.Common.DTOs.Categories;
using MediatR;

namespace MarketplaceSystem.Application.Features.Categories.Queries.GetAttributesByCategoryId
{
    public record GetAttributesByCategoryIdQuery(int CategoryId) : IRequest<List<CategoryAttributeDto>>;
}
