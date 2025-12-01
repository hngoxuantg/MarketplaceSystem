using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Application.Common.Interfaces.IServices;
using MarketplaceSystem.Domain.Entities.Identity_Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace MarketplaceSystem.Application.Features.Users.Commands.LockUser
{
    public class LockUserCommandHandler : IRequestHandler<LockUserCommand, bool>
    {
        private readonly UserManager<User> _userManager;
        private readonly ICurrentUserService _currentUserService;
        public LockUserCommandHandler(UserManager<User> userManager, ICurrentUserService currentUserService)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
        }
        public async Task<bool> Handle(LockUserCommand command, CancellationToken cancellationToken)
        {
            return await LockUserAsync(command.Id.ToString(), command.Request, cancellationToken);
        }
        private async Task<bool> LockUserAsync(string userId, LockUserRequest request, CancellationToken cancellation = default)
        {
            if (int.Parse(userId) == _currentUserService.UserId)
            {
                throw new BusinessRuleException("Người dùng không thể tự khóa chính mình!");
            }

            User user = await _userManager.FindByIdAsync(userId)
                ?? throw new NotFoundException("Không tìm thấy người dùng!");

            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
            {
                throw new BusinessRuleException("Người dùng đã bị khóa!");
            }
            else
            {
                await _userManager.SetLockoutEndDateAsync(user, request.Until.ToUniversalTime());
            }

            return true;
        }
    }
}
