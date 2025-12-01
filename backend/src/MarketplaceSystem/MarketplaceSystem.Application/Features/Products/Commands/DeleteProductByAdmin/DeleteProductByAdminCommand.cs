using MediatR;

namespace MarketplaceSystem.Application.Features.Products.Commands.DeleteProductByAdmin
{
    public record DeleteProductByAdminCommand(int Id) : IRequest<bool>;
}
