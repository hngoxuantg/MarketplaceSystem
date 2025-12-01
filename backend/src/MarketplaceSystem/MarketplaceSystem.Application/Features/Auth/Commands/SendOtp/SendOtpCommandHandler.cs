using MarketplaceSystem.Application.Common.DTOs.Emails;
using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Application.Common.Interfaces.IExternalServices.IMailServices;
using MarketplaceSystem.Domain.Entities.Identity_Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace MarketplaceSystem.Application.Features.Auth.Commands.SendOtp
{
    public class SendOtpCommandHandler : IRequestHandler<SendOtpCommand, string>
    {
        private readonly UserManager<User> _userManager;
        private readonly IMailService _mailService;
        public SendOtpCommandHandler(UserManager<User> userManager, IMailService mailService)
        {
            _userManager = userManager;
            _mailService = mailService;
        }
        public async Task<string> Handle(SendOtpCommand command, CancellationToken cancellationToken)
        {
            return await SendOtp(command.Request, cancellationToken);
        }
        private async Task<string> SendOtp(SendOtpRequest send, CancellationToken cancellation = default)
        {
            User? user = await _userManager.FindByEmailAsync(send.Email)
                ?? throw new NotFoundException("Email không tồn tại!");

            if (await _userManager.IsEmailConfirmedAsync(user))
                throw new BusinessRuleException("Email đã được xác thực!");

            var code = Convert.ToInt32(RandomNumberGenerator.GetInt32(100000, 999999)).ToString();

            EmailDto emailDto = new EmailDto
            {
                To = send.Email,
                Subject = "Mã xác thực OTP",
                Code = code
            };

            await _mailService.SendEmailAsync(emailDto, cancellation);

            return code;
        }
    }
}
