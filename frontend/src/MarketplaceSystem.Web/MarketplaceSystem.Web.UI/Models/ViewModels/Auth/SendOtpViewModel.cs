using System.ComponentModel.DataAnnotations;

namespace MarketplaceSystem.Web.UI.Models.ViewModels.Auth
{
    public class SendOtpViewModel
    {
        [Required(ErrorMessage = "Email là bắt buộc!")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ!")]
        public string Email { get; set; } = string.Empty;
    }
}
