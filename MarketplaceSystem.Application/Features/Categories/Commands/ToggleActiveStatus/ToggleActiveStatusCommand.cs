using MediatR;

namespace MarketplaceSystem.Application.Features.Categories.Commands.ToggleActiveStatus
{
    public record ToggleActiveStatusCommand(int Id) : IRequest<bool>;
}
