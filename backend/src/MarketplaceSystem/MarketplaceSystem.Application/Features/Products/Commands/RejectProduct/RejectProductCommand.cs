using MediatR;

namespace MarketplaceSystem.Application.Features.Products.Commands.RejectProduct
{
    public record RejectProductCommand(int Id, RejectProductRequest Request) : IRequest<bool>;
}
