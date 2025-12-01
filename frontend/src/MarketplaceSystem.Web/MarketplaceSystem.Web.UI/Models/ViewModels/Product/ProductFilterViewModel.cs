namespace MarketplaceSystem.Web.UI.Models.ViewModels.Product
{
    /// <summary>
    /// ViewModel cho filter/search sản phẩm - dùng chung
    /// </summary>
    public class ProductFilterViewModel
    {
        public int? CategoryId { get; set; }
        public int? Location { get; set; }
        public string? Search { get; set; }
        public int PageSize { get; set; } = 20;
        public int PageNumber { get; set; } = 1;
    }
}
