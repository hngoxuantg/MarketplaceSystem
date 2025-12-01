namespace MarketplaceSystem.Application.Common.DTOs.Categories
{
    public class RootCategoryCardDto
    {
        public int TotalCount { get; set; }

        public List<RootCategoryItemListDto> Items { get; set; }
    }
    public class RootCategoryItemListDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public string? Icon { get; set; }

        public int? DisplayOrder { get; set; }
    }
}
