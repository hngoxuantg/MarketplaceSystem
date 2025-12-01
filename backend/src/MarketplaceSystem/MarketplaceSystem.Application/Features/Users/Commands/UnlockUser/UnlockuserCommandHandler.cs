using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Application.Common.Interfaces.IServices;
using MarketplaceSystem.Domain.Entities.Identity_Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace MarketplaceSystem.Application.Features.Users.Commands.UnlockUser
{
    public class UnlockuserCommandHandler : IRequestHandler<UnlockUserCommand, bool>
    {
        private readonly UserManager<User> _userManager;
        private readonly ICurrentUserService _currentUserService;

        public UnlockuserCommandHandler(UserManager<User> userManager, ICurrentUserService currentUserService)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(UnlockUserCommand command, CancellationToken cancellationToken)
        {
            return await UnlockUserAsync(command.Id.ToString(), cancellationToken);
        }
        private async Task<bool> UnlockUserAsync(string id, CancellationToken cancellation = default)
        {
            if (int.Parse(id) == _currentUserService.UserId)
            {
                throw new BusinessRuleException("Người dùng không thể tự mở khóa chính mình!");
            }

            User user = await _userManager.FindByIdAsync(id)
                ?? throw new NotFoundException("Không tìm thấy người dùng!");

            if (user.LockoutEnd == null || user.LockoutEnd <= DateTimeOffset.UtcNow)
            {
                throw new BusinessRuleException("Người dùng không bị khóa!");
            }

            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);

            return true;
        }
    }
}
