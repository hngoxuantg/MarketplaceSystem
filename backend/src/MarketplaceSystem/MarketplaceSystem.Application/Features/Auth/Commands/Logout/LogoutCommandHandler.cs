using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Domain.Entities.Identity_Auth;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;

namespace MarketplaceSystem.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public LogoutCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(LogoutCommand command, CancellationToken cancellationToken)
        {
            return await LogoutAsync(command.RefreshToken, cancellationToken);
        }

        private async Task<bool> LogoutAsync(string? refreshToken, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(refreshToken))
                throw new ValidatorException("Không tìm thấy RefreshToken ở cookie!");

            RefreshToken? oldToken = await GetRefreshTokenAsync(refreshToken, cancellationToken);

            if (!oldToken.IsActive)
                throw new ValidatorException("RefreshToken không hợp lệ hoặc đã hết hạn!");

            oldToken.Revoke();

            await _unitOfWork.RefreshTokenRepository.UpdateAsync(oldToken, cancellationToken);

            return true;
        }

        private async Task<RefreshToken> GetRefreshTokenAsync(string? refreshToken, CancellationToken cancellationToken = default)
        {
            RefreshToken? oldToken = await _unitOfWork.RefreshTokenRepository.GetOneAsync<RefreshToken>(
                filter: r => r.Token == refreshToken, cancellation: cancellationToken)
                    ?? throw new NotFoundException("RefreshToken không tồn tại!");

            return oldToken;
        }
    }
}
