namespace MarketplaceSystem.Web.UI.Models.ViewModels.Shared
{
    /// <summary>
    /// Location item cho dropdown/filter - dùng chung cho nhiều page
    /// </summary>
    public class LocationItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
