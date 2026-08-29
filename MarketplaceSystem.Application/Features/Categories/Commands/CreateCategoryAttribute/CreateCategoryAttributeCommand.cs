using MarketplaceSystem.Application.Common.DTOs.Categories;
using MediatR;

namespace MarketplaceSystem.Application.Features.Categories.Commands.CreateCategoryAttribute
{
    public record CreateCategoryAttributeCommand(
        int CategoryId,
        CreateCategoryAttributeRequest Request) : IRequest<CategoryAttributeDto>;
}
