namespace MarketplaceSystem.Web.UI.Models.ViewModels.Product
{
    public class ProductAttributeValueViewModel
    {
        public int Id { get; set; }
        public string AttributeName { get; set; }
        public string? TextValue { get; set; }
        public decimal? NumberValue { get; set; }
        public bool? BooleanValue { get; set; }
        public DateTime? DateValue { get; set; }
        public string? SelectsValue { get; set; }
    }
}
