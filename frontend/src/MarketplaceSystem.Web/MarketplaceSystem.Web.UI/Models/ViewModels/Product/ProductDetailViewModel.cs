using System.Globalization;

namespace MarketplaceSystem.Web.UI.Models.ViewModels.Product
{
    /// <summary>
    /// ViewModel cho View - có các thuộc tính đã format và helper methods
    /// </summary>
    public class ProductDetailViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ProductStatus { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public ProductImagesViewModel Images { get; set; } = new();
        public List<ProductAttributeValueViewModel> AttributeValues { get; set; } = new();
        public SellerViewModel Seller { get; set; } = new();


        public string FormattedPrice => Price.ToString("N0", new CultureInfo("vi-VN")) + " đ";

        public string TimeAgo
        {
            get
            {
                var timeSpan = DateTime.Now - CreatedAt;
                if (timeSpan.TotalMinutes < 1) return "Vừa xong";
                if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes} phút trước";
                if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours} giờ trước";
                if (timeSpan.TotalDays < 30) return $"{(int)timeSpan.TotalDays} ngày trước";
                if (timeSpan.TotalDays < 365) return $"{(int)(timeSpan.TotalDays / 30)} tháng trước";
                return $"{(int)(timeSpan.TotalDays / 365)} năm trước";
            }
        }

        public bool IsAvailable => ProductStatus == "Available" && Quantity > 0;

        public string StatusText => ProductStatus switch
        {
            "Available" => "Còn hàng",
            "Sold" => "Đã bán",
            "Hidden" => "Đã ẩn",
            _ => ProductStatus
        };

        public string ConditionText => Condition switch
        {
            "New" => "Mới",
            "LikeNew" => "Như mới",
            "Used" => "Đã sử dụng",
            _ => Condition
        };

        // Helper methods
        public bool HasMultipleImages() => Images?.Images?.Count > 1;
        public bool HasAttributes() => AttributeValues?.Any() == true;
    }
}
