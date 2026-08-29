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

namespace MarketplaceSystem.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly UserManager<User> _userManager;
        private readonly AppSettings _appSettings;
        private readonly ICurrentUserService _currentUserService;
        public LoginCommandHandler(
            IUnitOfWork unitOfWork,
            IJwtTokenService jwtTokenService,
            UserManager<User> userManager,
            IOptions<AppSettings> appsettings,
            ICurrentUserService currentUserService
            )
        {
            _unitOfWork = unitOfWork;
            _jwtTokenService = jwtTokenService;
            _userManager = userManager;
            _appSettings = appsettings.Value;
            _currentUserService = currentUserService;
        }
        public async Task<AuthDto> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            return await LoginAsync(command.Request, _currentUserService.DeviceInfo, _currentUserService.IpAddress, command.Role, cancellationToken);
        }
        private async Task<AuthDto> LoginAsync(
            LoginRequest request,
            string? deviceInfo,
            string? ipAddress,
            string role,
            CancellationToken cancellationToken = default)
        {
            User user = await ValidateForLogin(request, role, cancellationToken);
            user.UpdateLastLogin();

            string accessToken = await _jwtTokenService.GenerateJwtTokenAsync(user, cancellationToken);
            string refreshToken = _jwtTokenService.GenerateRefreshToken();

            RefreshToken newToken = new RefreshToken(
                user.Id, refreshToken,
                DateTime.UtcNow.AddDays(_appSettings.JwtConfig.RefreshTokenExpirationDays),
                deviceInfo,
                ipAddress);

            _unitOfWork.RefreshTokenRepository.AddEntity(newToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new AuthDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        private async Task<User> ValidateForLogin(LoginRequest request, string role, CancellationToken cancellation = default)
        {
            if (_currentUserService.IsAuthenticated)
                throw new ValidatorException("Người dùng đã xác thực!");

            User? user = await _userManager.FindByEmailAsync(
                request.Email) ?? throw new NotFoundException($"Email không tồn tại!");

            if (await _userManager.IsLockedOutAsync(user))
                throw new ForbiddenAccessException("Tài khoản đã bị khóa!");

            IList<string> roles = await _userManager.GetRolesAsync(user);

            if (!roles.Contains(role))
                throw new ForbiddenAccessException("Người dùng không có quyền truy cập!");

            if (!await _userManager.IsEmailConfirmedAsync(user))
                throw new BusinessRuleException("Email chưa được xác thực!");

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
            {
                await _userManager.AccessFailedAsync(user);
                throw new ValidatorException(nameof(LoginRequest.ReferenceEquals), "Mật khẩu không đúng!");
            }
            else
            {
                await _userManager.ResetAccessFailedCountAsync(user);
            }

            return user;
        }
    }
}
