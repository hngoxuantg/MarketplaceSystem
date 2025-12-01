using MarketplaceSystem.Application.Common.DTOs.Auths;
using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Application.Common.Interfaces.IExternalServices.ITokenServices;
using MarketplaceSystem.Application.Common.Interfaces.IServices;
using MarketplaceSystem.Common.Options;
using MarketplaceSystem.Domain.Entities.Identity_Auth;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace MarketplaceSystem.Application.Features.Auth.Commands.Refresh
{
    public class RefreshCommandHandler : IRequestHandler<RefreshCommand, AuthDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly AppSettings _appSettings;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<User> _userManager;
        public RefreshCommandHandler(
            IUnitOfWork unitOfWork,
            IJwtTokenService jwtTokenService,
            IOptions<AppSettings> appSettings,
            ICurrentUserService currentUserService,
            UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _jwtTokenService = jwtTokenService;
            _appSettings = appSettings.Value;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }
        public async Task<AuthDto> Handle(RefreshCommand command, CancellationToken cancellationToken)
        {
            (string accessToken, string newRefreshToken) = await RefreshAsync(
                command.RefreshToken,
                _currentUserService.DeviceInfo,
                _currentUserService.IpAddress,
                command.Role,
                cancellationToken);

            return new AuthDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken
            };
        }
        private async Task<(string, string)> RefreshAsync(
            string? refreshToken,
            string? deviceInfo,
            string? ipAddress,
            string role,
            CancellationToken cancellationToken = default)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                RefreshToken? oldToken = await _unitOfWork.RefreshTokenRepository.GetOneAsync<RefreshToken>(
                    filter: r => r.Token == refreshToken,
                    cancellation: cancellationToken) ?? throw new NotFoundException("RefreshToken không tồn tại");

                if (!oldToken.IsActive)
                    throw new ValidatorException("RefreshToken không hợp lệ hoặc đã hết hạn");

                User? user = await _unitOfWork.UserRepository.GetByIdAsync(
                    oldToken.UserId,
                    cancellation: cancellationToken)
                    ?? throw new NotFoundException("User không tồn tại");

                IList<string> userRoles = await _userManager.GetRolesAsync(user);

                if (!userRoles.Contains(role))
                    throw new ForbiddenAccessException("Bạn kkhoong có quyền truy cập vào tài nguyên này!");

                oldToken.Revoke();

                string accessToken = await _jwtTokenService.GenerateJwtTokenAsync(user, cancellationToken);
                string newRefreshToken = _jwtTokenService.GenerateRefreshToken();

                RefreshToken newRefreshTokenEntity = new RefreshToken(
                    user.Id, newRefreshToken,
                    DateTime.UtcNow.AddDays(_appSettings.JwtConfig.RefreshTokenExpirationDays),
                    deviceInfo,
                    ipAddress);

                _unitOfWork.RefreshTokenRepository.AddEntity(newRefreshTokenEntity);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return (accessToken, newRefreshToken);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
