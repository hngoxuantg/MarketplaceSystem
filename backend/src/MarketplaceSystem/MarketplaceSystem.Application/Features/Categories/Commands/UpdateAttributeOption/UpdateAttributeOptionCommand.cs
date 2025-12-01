using MarketplaceSystem.Application.Common.DTOs.Categories;
using MediatR;

namespace MarketplaceSystem.Application.Features.Categories.Commands.UpdateAttributeOption
{
    public record UpdateAttributeOptionCommand(
        int CategoryId,
        int AttributeId,
        int OptionId,
        UpdateAttributeOptionRequest Request) : IRequest<AttributeOptionDto>;
}
