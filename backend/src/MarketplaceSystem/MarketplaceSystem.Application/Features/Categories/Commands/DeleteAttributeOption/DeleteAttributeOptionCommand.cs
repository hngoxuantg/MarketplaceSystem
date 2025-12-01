using MediatR;

namespace MarketplaceSystem.Application.Features.Categories.Commands.DeleteAttributeOption
{
    public record DeleteAttributeOptionCommand(int CategoryId, int AttributeId, int OptionId) : IRequest<bool>;
}
