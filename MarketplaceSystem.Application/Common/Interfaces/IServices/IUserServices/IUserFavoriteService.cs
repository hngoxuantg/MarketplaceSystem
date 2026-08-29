namespace MarketplaceSystem.Application.Common.Interfaces.IServices.IUserServices
{
    public interface IUserFavoriteService
    {
        Task<bool> RemoveFavoriteAsync(int userId, int productId, CancellationToken cancellation = default);
    }
}
