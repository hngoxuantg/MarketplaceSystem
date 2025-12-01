using System.ComponentModel.DataAnnotations;

namespace MarketplaceSystem.Web.UI.Models.ViewModels.Post
{
    public class CreatePostViewModel
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề tin đăng")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Tiêu đề phải từ 10 đến 100 ký tự")]
        public string Title { get; set; }


        [StringLength(3000, MinimumLength = 30, ErrorMessage = "Mô tả phải từ 30 đến 3000 ký tự")]
        public string? Description { get; set; }


        [Required(ErrorMessage = "Vui lòng nhập giá")]
        [Range(1000, 1000000000000, ErrorMessage = "Giá phải từ 1.000đ đến 1.000.000.000.000đ")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn tình trạng sản phẩm")]
        public int Condition { get; set; }

        public int Quantity { get; set; } = 1;

        [Required(ErrorMessage = "Vui lòng chọn tỉnh/thành phố")]
        public int Location { get; set; }

        public List<ProductAttributeValueRequest>? AttributeValues { get; set; }
    }

    public class ProductAttributeValueRequest
    {
        public int CategoryAttributeId { get; set; }

        public string? TextValue { get; set; }

        public decimal? NumberValue { get; set; }

        public bool? BooleanValue { get; set; }

        public DateTime? DateValue { get; set; }

        public string? SelectValues { get; set; }
    }
}