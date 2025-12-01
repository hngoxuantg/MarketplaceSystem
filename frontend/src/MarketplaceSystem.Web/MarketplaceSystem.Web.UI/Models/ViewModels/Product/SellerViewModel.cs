namespace MarketplaceSystem.Web.UI.Models.ViewModels.Product
{
    /// <summary>
    /// ViewModel cho thông tin người bán
    /// </summary>
    public class SellerViewModel
    {
        public int UserId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? AvatarUrl { get; set; }
        public int TotalProducts { get; set; }
        public double Rating { get; set; }
        public DateTime JoinedDate { get; set; }

        // Computed properties
        public string AvatarUrlOrDefault => !string.IsNullOrEmpty(AvatarUrl)
            ? AvatarUrl
            : "/images/default-avatar.png";

        public string JoinedText
        {
            get
            {
                var timeSpan = DateTime.Now - JoinedDate;
                if (timeSpan.TotalDays < 30) return "Mới tham gia";
                if (timeSpan.TotalDays < 365) return $"{(int)(timeSpan.TotalDays / 30)} tháng";
                return $"{(int)(timeSpan.TotalDays / 365)} năm";
            }
        }

        public string RatingText => Rating > 0 ? Rating.ToString("0.0") : "Chưa có đánh giá";
        public bool HasRating => Rating > 0;
    }
}
