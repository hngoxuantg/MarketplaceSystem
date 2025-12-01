using MediatR;

namespace MarketplaceSystem.Application.Features.Users.Commands.RemoveFavorite
{
    public record RemoveFavoriteCommand(int UserId, int ProductId) : IRequest<bool>;
}
