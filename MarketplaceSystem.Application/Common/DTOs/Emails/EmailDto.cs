using Microsoft.AspNetCore.Http;

namespace MarketplaceSystem.Application.Common.DTOs.Emails
{
    public class EmailDto
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Code { get; set; }
        public IFormFile? FormFile { get; set; }
    }
}
