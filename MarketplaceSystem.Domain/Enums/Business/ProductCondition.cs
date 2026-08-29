using System.ComponentModel.DataAnnotations;

namespace MarketplaceSystem.Domain.Enums.Business
{
    public enum ProductCondition
    {
        [Display(Name = "Mới tinh")]
        New = 1,

        [Display(Name = "Như mới")]
        LikeNew = 2,

        [Display(Name = "Đã sử dụng")]
        Used = 3,

        [Display(Name = "Tân trang lại")]
        Refurbished = 4,

        [Display(Name = "Hỏng")]
        Broken = 5
    }
}
