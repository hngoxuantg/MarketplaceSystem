using System.ComponentModel.DataAnnotations;

namespace MarketplaceSystem.Domain.Enums.Business
{
    public enum SellerVerificationStatus
    {
        [Display(Name = "Không phải người bán")]
        NotSeller = 0,

        [Display(Name = "Đang chờ duyệt")]
        Pending = 1,

        [Display(Name = "Đã duyệt")]
        Approved = 2,

        [Display(Name = "Bị từ chối")]
        Rejected = 3,

        [Display(Name = "Bị đình chỉ")]
        Suspended = 4
    }
}
