using MediatR;

namespace MarketplaceSystem.Application.Features.Products.Commands.IncrementViewCount
{
    public record IncrementViewCountCommand(int Id) : IRequest<bool>;
}
