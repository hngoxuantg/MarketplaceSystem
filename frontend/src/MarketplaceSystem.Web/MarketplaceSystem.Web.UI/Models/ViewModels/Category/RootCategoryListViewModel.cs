namespace MarketplaceSystem.Web.UI.Models.ViewModels.Category
{
    public class RootCategoryListViewModel
    {
        public int TotalCount { get; set; }
        public List<RootCategoryItemList>? Items { get; set; }
    }
    public class RootCategoryItemList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public int DisplayOrder { get; set; }
    }
}
