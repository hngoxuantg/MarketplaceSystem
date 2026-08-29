using System.ComponentModel.DataAnnotations;

namespace MarketplaceSystem.Domain.Enums.Business
{
    public enum UserProfileGender
    {
        [Display(Name = "Nam")]
        Male = 1,

        [Display(Name = "Nữ")]
        Female = 2,

        [Display(Name = "Khác")]
        Other = 3
    }
}
