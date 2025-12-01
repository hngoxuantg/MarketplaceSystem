using System.ComponentModel.DataAnnotations;

namespace MarketplaceSystem.Application.Features.Products.Commands.RejectProduct
{
    public class RejectProductRequest
    {
        [MaxLength(500, ErrorMessage = "Không vượt quá 500 ký tự!")]
        public string? RejectionReason { get; set; }
    }
}
