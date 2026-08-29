using MediatR;

namespace MarketplaceSystem.Application.Features.Products.Commands.MarkAsSold
{
    public record MarkAsSoldCommand(int ProductId) : IRequest<bool>;
}
