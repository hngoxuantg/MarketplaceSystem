using MarketplaceSystem.Domain.Entities.Identity_Auth;

namespace MarketplaceSystem.Application.Common.Interfaces.IExternalServices.ITokenServices
{
    public interface IJwtTokenService
    {
        Task<string> GenerateJwtTokenAsync(User user, CancellationToken cancellation = default);
        string GenerateRefreshToken();
    }
}
