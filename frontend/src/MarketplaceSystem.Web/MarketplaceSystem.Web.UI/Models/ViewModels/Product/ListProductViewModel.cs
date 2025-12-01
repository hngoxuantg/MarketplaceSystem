using System.Globalization;

namespace MarketplaceSystem.Web.UI.Models.ViewModels.Product
{
    /// <summary>
    /// ViewModel cho sản phẩm trong danh sách - có format helpers
    /// Dùng chung cho: HomePage, Search, Category, User Products, etc.
    /// </summary>
    public class ListProductViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public string Condition { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string ProductStatus { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Computed properties
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
                return CreatedAt.ToString("dd/MM/yyyy");
            }
        }

        public string ConditionText => Condition switch
        {
            "New" => "Mới",
            "LikeNew" => "Như mới",
            "Used" => "Đã sử dụng",
            _ => Condition
        };

        public string ImageUrlOrDefault => !string.IsNullOrEmpty(ImageUrl)
            ? ImageUrl
            : "/images/no-image.png";
    }
}
