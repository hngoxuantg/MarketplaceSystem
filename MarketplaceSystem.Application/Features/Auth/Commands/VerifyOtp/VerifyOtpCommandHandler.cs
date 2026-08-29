using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Domain.Entities.Identity_Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace MarketplaceSystem.Application.Features.Auth.Commands.VerifyOtp
{
    public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, bool>
    {
        private readonly UserManager<User> _userManager;
        public VerifyOtpCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public async Task<bool> Handle(VerifyOtpCommand command, CancellationToken cancellationToken)
        {
            return await VerifyOtp(command.Request.Email, cancellationToken);
        }
        public async Task<bool> VerifyOtp(string email, CancellationToken cancellation = default)
        {
            User? user = await _userManager.FindByEmailAsync(email)
                ?? throw new NotFoundException("Email không tồn tại!");

            if (await _userManager.IsEmailConfirmedAsync(user))
                throw new BusinessRuleException("Email đã được xác thực!");

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            IdentityResult result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
                throw new Exception($"Failed to confirm email: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            return true;
        }
    }
}
