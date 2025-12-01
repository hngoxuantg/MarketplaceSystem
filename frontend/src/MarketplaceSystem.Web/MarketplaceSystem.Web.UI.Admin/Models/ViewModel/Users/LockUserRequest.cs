using System.ComponentModel.DataAnnotations;

namespace MarketplaceSystem.Web.UI.Admin.Models.ViewModel.Users;

public class LockUserRequest
{
    [Required(ErrorMessage = "Thời gian khóa là bắt buộc")]
    public DateTime Until { get; set; }
}
