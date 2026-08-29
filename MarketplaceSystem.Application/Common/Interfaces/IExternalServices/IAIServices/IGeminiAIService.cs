using MarketplaceSystem.Domain.Enums.Business;
using Microsoft.AspNetCore.Http;

namespace MarketplaceSystem.Application.Common.Interfaces.IExternalServices.IAIServices
{
    public interface IGeminiAIService
    {
        Task<(ContentWarningFlag? Flag, string? Detail)> CheckProductContentAsync(
            string title,
            string description,
            List<IFormFile> images,
            CancellationToken cancellation = default);
    }
}
