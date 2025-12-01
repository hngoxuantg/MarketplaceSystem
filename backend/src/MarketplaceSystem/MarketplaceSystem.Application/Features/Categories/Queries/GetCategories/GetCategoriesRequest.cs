using MarketplaceSystem.Common.Models.Pagination;

namespace MarketplaceSystem.Application.Features.Categories.Queries.GetCategories
{
    public class GetCategoriesRequest : PaginatedRequest
    {
        public bool? IsActive { get; set; }
    }
}
