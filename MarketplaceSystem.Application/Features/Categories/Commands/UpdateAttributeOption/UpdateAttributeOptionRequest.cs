namespace MarketplaceSystem.Application.Features.Categories.Commands.UpdateAttributeOption
{
    public class UpdateAttributeOptionRequest
    {
        public string Value { get; set; }
        public string DisplayText { get; set; }
        public bool IsActive { get; set; }
    }
}
