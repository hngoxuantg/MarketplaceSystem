namespace MarketplaceSystem.Web.UI.Models.ViewModels.Product
{
    /// <summary>
    /// Wrapper cho danh sách sản phẩm với pagination
    /// Dùng chung cho: HomePage, Search, Category, User Products, etc.
    /// </summary>
    public class ProductListViewModel
    {
        public List<ListProductViewModel> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }

        // Helper properties
        public bool IsEmpty => !Items.Any();
        public string PaginationInfo => $"Hiển thị {Items.Count} / {TotalCount} sản phẩm";
    }
}
