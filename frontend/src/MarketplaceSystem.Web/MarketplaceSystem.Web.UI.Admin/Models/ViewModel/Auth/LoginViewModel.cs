using System.ComponentModel.DataAnnotations;

namespace MarketplaceSystem.Web.UI.Admin.Models.ViewModel.Auth
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email là bắt buộc!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password là bắt buộc!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string? ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }
}