using System.ComponentModel.DataAnnotations;

namespace MarketplaceSystem.Domain.Enums.Business
{
    public enum AttributeType
    {
        [Display(Name = "Văn bản")]
        Text = 1,

        [Display(Name = "Số")]
        Number = 2,

        [Display(Name = "Lựa chọn")]
        Select = 3,

        [Display(Name = "Có không - đúng sai")]
        Boolean = 5,

        [Display(Name = "Ngày tháng")]
        Date = 6
    }
}
