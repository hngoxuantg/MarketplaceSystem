using MediatR;

namespace MarketplaceSystem.Application.Features.Products.Commands.ApproveProduct
{
    public record ApproveProductCommand(int Id) : IRequest<bool>;
}
