using System.ComponentModel.DataAnnotations;

namespace MarketplaceSystem.Domain.Enums.Business
{
    public enum ProductStatus
    {
        [Display(Name = "Nháp")]
        Draft = 1,

        [Display(Name = "Chờ duyệt")]
        PendingApproval = 2,

        [Display(Name = "Đang bán")]
        Active = 3,

        [Display(Name = "Đã bán")]
        Sold = 4,

        [Display(Name = "Hết hạn")]
        Expired = 5,

        [Display(Name = "Bị cấm")]
        Banned = 6,

        [Display(Name = "Bị từ chối")]
        Rejected = 8
    }
}