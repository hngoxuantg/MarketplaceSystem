using System.ComponentModel.DataAnnotations;

namespace MarketplaceSystem.Domain.Enums.Business
{
    public enum MessageType
    {
        [Display(Name = "Văn bản")]
        Text = 1,

        [Display(Name = "Hình ảnh")]
        Image = 2,

        [Display(Name = "Video")]
        Video = 4
    }
}
