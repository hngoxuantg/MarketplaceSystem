using MediatR;

namespace MarketplaceSystem.Application.Features.Users.Commands.AddFavorite
{
    public record AddFavoriteCommand(int UserId, int ProductId) : IRequest<bool>;
}
