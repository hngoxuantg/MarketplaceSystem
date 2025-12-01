using System.ComponentModel.DataAnnotations;

namespace MarketplaceSystem.Web.UI.Models.ViewModels.Auth
{
    public class VerifyOtpViewModel
    {
        public string? Email { get; set; }

        [Required(ErrorMessage = "Mã OTP là bắt buộc!")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Mã OTP phải có 6 ký tự!")]
        public string Otp { get; set; } = string.Empty;
    }
}
