using System.ComponentModel.DataAnnotations;

namespace MarketplaceSystem.Web.UI.Models.ViewModels.Chat
{
    public class SendMessageViewModel
    {
        [Required(ErrorMessage = "Người nhận không được để trống")]
        public int ReceiverId { get; set; }
        
        public string? Content { get; set; }
        
        public IFormFile? Image { get; set; }
    }
}
