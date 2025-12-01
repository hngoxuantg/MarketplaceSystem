using MarketplaceSystem.Common.Enums;
using MarketplaceSystem.Common.Models.Pagination;

namespace MarketplaceSystem.Application.Features.Products.Queries.GetProductsByFilter
{
    public class ProductsByFilterRequest : PaginatedRequest
    {
        public VietnamProvince? Location { get; set; }

        public int? CategoryId { get; set; }
    }
}
