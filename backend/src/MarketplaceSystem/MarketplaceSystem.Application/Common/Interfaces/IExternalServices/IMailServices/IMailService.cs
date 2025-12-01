using MarketplaceSystem.Application.Common.DTOs.Emails;

namespace MarketplaceSystem.Application.Common.Interfaces.IExternalServices.IMailServices
{
    public interface IMailService
    {
        Task SendEmailAsync(EmailDto emailDto, CancellationToken cancellation = default);
    }
}
