using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Application.Common.Interfaces.IServices;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Entities.Identity_Auth;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;

namespace MarketplaceSystem.Application.Features.Users.Commands.RemoveFavorite
{
    public class RemoveFavoriteCommandHandler : IRequestHandler<RemoveFavoriteCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        public RemoveFavoriteCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }
        public async Task<bool> Handle(RemoveFavoriteCommand command, CancellationToken cancellationToken)
        {
            return await RemoveFavoriteAsync(command.UserId, command.ProductId, cancellationToken);
        }
        private async Task<bool> RemoveFavoriteAsync(int userId, int productId, CancellationToken cancellation = default)
        {
            ValidateUserExists(userId);

            FavoriteProduct? favoriteProduct = await _unitOfWork.FavoriteProductRepository.GetOneUntrackedAsync<FavoriteProduct>(
                filter: fp => fp.ProductId == productId && fp.UserId == userId,
                cancellation: cancellation);

            if (favoriteProduct == null)
                throw new NotFoundException("Sản phẩm không tồn tại trong danh sách yêu thích của bạn!");

            User? user = await _unitOfWork.UserRepository.GetByIdAsync(userId, cancellation);

            user.RemoveFavorite(productId);

            await _unitOfWork.SaveChangesAsync(cancellation);

            return true;
        }

        private void ValidateUserExists(int userId)
        {
            if (userId != _currentUser.UserId)
            {
                throw new ForbiddenAccessException("Bạn không được phép truy cập vào tài nguyên này!");
            }
        }
    }
}
