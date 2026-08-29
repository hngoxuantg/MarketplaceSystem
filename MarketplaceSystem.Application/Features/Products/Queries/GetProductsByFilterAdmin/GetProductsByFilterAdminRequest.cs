using MarketplaceSystem.Common.Models.Pagination;
using System.ComponentModel.DataAnnotations;

namespace MarketplaceSystem.Application.Features.Products.Queries.GetProductsByFilterAdmin
{
    public class GetProductsByFilterAdminRequest : PaginatedRequest
    {
        public ProductAdminStatusFilter? Status { get; set; }

        public int? CategoryId { get; set; }

        public ProductAdminSortBy? SortBy { get; set; }
    }
    public enum ProductAdminStatusFilter
    {
        [Display(Name = "Tất cả")]
        All = 0,

        [Display(Name = "Nháp")]
        Draft = 1,

        [Display(Name = "Chờ duyệt")]
        PendingApproval = 2,

        [Display(Name = "Đang bán")]
        Active = 3,

        [Display(Name = "Đã bán")]
        Sold = 4,

        [Display(Name = "Hết hạn")]
        Expired = 5,

        [Display(Name = "Bị cấm")]
        Banned = 6,

        [Display(Name = "Bị từ chối")]
        Rejected = 8,

        [Display(Name = "Đã duyệt")]
        Approved = 99
    }

    public enum ProductAdminSortBy
    {
        [Display(Name = "Mới nhất")]
        CreatedAtAsc = 1,

        [Display(Name = "Cũ nhất")]
        CreatedAtDesc = 2,

        [Display(Name = "Giá tăng dần")]
        PriceAsc = 3,

        [Display(Name = "Giá giảm dần")]
        PriceDesc = 4
    }
}
