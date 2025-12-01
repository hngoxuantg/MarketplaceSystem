using System.ComponentModel.DataAnnotations;

namespace MarketplaceSystem.Web.UI.Models.ViewModels.Auth
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Họ là bắt buộc!")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc!")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Thành phố là bắt buộc")]
        public int Location { get; set; }

        [Required(ErrorMessage = "Ngày sinh là bắt buộc!")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Giới tính là bắt buộc!")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc!")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp!")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public string? ReturnUrl { get; set; }
    }
}