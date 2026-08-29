using MediatR;

namespace MarketplaceSystem.Application.Features.Categories.Commands.DeleteCategoryAttribute
{
    public record DeleteCategoryAttributeCommand(int CategoryId, int AttributeId) : IRequest<bool>;
}
