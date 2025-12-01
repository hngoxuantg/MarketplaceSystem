using MarketplaceSystem.Application.Common.DTOs.Categories;
using MediatR;

namespace MarketplaceSystem.Application.Features.Categories.Commands.UpdateCategoryAttribute
{
    public record UpdateCategoryAttributeCommand(
        int CategoryId,
        int AttributeId,
        UpdateCategoryAttributeRequest Request) : IRequest<CategoryAttributeDto>;
}
