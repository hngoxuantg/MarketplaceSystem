using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Application.Common.Interfaces.IServices;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Entities.Identity_Auth;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;

namespace MarketplaceSystem.Application.Features.Users.Commands.AddFavorite
{
    public class AddFavoriteCommandHandler : IRequestHandler<AddFavoriteCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        public AddFavoriteCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }
        public async Task<bool> Handle(AddFavoriteCommand command, CancellationToken cancellationToken)
        {
            return await AddFavoriteAsync(command.ProductId, command.UserId, cancellationToken);
        }
        private async Task<bool> AddFavoriteAsync(int productId, int userId, CancellationToken cancellation = default)
        {
            ValidateUserExists(userId);

            FavoriteProduct? existingFavorite = await _unitOfWork.FavoriteProductRepository.GetOneUntrackedAsync<FavoriteProduct>(
                filter: fp => fp.ProductId == productId && fp.UserId == userId);

            if (existingFavorite != null)
                throw new ValidatorException("Sản phẩm đã tồn tại trong danh sách yêu thích của bạn!");

            User? user = await _unitOfWork.UserRepository.GetByIdAsync(userId, cancellation);

            user.AddFavorite(productId);
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
