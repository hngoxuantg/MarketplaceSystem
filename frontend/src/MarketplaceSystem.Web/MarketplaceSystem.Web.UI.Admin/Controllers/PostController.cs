using MarketplaceSystem.Web.UI.Admin.Interfaces;
using MarketplaceSystem.Web.UI.Admin.Models.ViewModel.Products;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceSystem.Web.UI.Admin.Controllers
{
    public class PostController : BaseController
    {
        private readonly ILogger<PostController> _logger;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IEnumService _enumService;

        public PostController(
            ILogger<PostController> logger,
            IProductService productService,
            ICategoryService categoryService,
            IEnumService enumService)
        {
            _logger = logger;
            _productService = productService;
            _categoryService = categoryService;
            _enumService = enumService;
        }

        // GET: Post/Pending - Bài đăng chờ duyệt (Status = 2)
        public async Task<IActionResult> Pending(ProductFilterViewModel filter, CancellationToken cancellation = default)
        {
            filter.Status = 2; // Pending
            return await LoadPostsView(filter, "Pending", cancellation);
        }

        // GET: Post/Approved - Bài đăng đã duyệt (Status = 99)
        public async Task<IActionResult> Approved(ProductFilterViewModel filter, CancellationToken cancellation = default)
        {
            filter.Status = 99; // Approved
            return await LoadPostsView(filter, "Approved", cancellation);
        }

        // GET: Post/Rejected - Bài đăng đã từ chối (Status = 8)
        public async Task<IActionResult> Rejected(ProductFilterViewModel filter, CancellationToken cancellation = default)
        {
            filter.Status = 8; // Rejected
            return await LoadPostsView(filter, "Rejected", cancellation);
        }

        // GET: Post/All - Tất cả bài đăng (Status = null)
        public async Task<IActionResult> All(ProductFilterViewModel filter, CancellationToken cancellation = default)
        {
            filter.Status = null; // All
            return await LoadPostsView(filter, "All", cancellation);
        }

        // Helper method to load posts
        private async Task<IActionResult> LoadPostsView(ProductFilterViewModel filter, string viewName, CancellationToken cancellation)
        {
            try
            {
                // Get products
                var productsResult = await _productService.GetProductsAsync(filter, cancellation);

                // Get categories for filter (top 100)
                var categoriesResult = await _categoryService.GetCategoriesAsync(1, 100, null, true, cancellation);

                // Get sort options
                var sortOptionsResult = await _enumService.GetSortOptionsAsync(cancellation);

                ViewBag.Categories = categoriesResult?.Data?.Data ?? new List<Models.ViewModel.Categories.CategoryViewModel>();
                ViewBag.SortOptions = sortOptionsResult?.Data ?? new List<Models.ViewModel.Shared.SortOption>();
                ViewBag.Filter = filter;
                ViewBag.Products = productsResult?.Data;

                return View(viewName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading {ViewName} posts", viewName);
                TempData["Error"] = "Có lỗi xảy ra khi tải danh sách bài đăng.";
                ViewBag.Products = null;
                return View(viewName);
            }
        }

        // GET: Post/Details/5 - Chi tiết bài đăng
        public IActionResult Details(int id)
        {
            // TODO: Lấy chi tiết bài đăng từ API
            // var post = await _postService.GetPostByIdAsync(id);
            ViewBag.PostId = id;
            return View();
        }

        // POST: Post/Approve/5 - Duyệt bài đăng
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Approve(int id, [FromBody] ApproveRequest request, CancellationToken cancellation = default)
        {
            try
            {
                var result = await _productService.ApproveProductAsync(id, request?.Note, cancellation);

                if (result == null || !result.Success)
                {
                    return Json(new { success = false, message = result?.Message ?? "Duyệt bài đăng thất bại." });
                }

                return Json(new { success = true, message = "Đã duyệt bài đăng thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving product {ProductId}", id);
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // POST: Post/Reject/5 - Từ chối bài đăng
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Reject(int id, [FromBody] RejectRequest request, CancellationToken cancellation = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.Reason))
                {
                    return Json(new { success = false, message = "Lý do từ chối không được để trống!" });
                }

                var result = await _productService.RejectProductAsync(id, request.Reason, cancellation);

                if (result == null || !result.Success)
                {
                    return Json(new { success = false, message = result?.Message ?? "Từ chối bài đăng thất bại." });
                }

                return Json(new { success = true, message = "Đã từ chối bài đăng!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting product {ProductId}", id);
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // POST: Post/Delete/5 - Xóa bài đăng
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Delete(int id, [FromBody] DeleteRequest request, CancellationToken cancellation = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.Reason))
                {
                    return Json(new { success = false, message = "Lý do xóa không được để trống!" });
                }

                var result = await _productService.DeleteProductAsync(id, request.Reason, cancellation);

                if (result == null || !result.Success)
                {
                    return Json(new { success = false, message = result?.Message ?? "Xóa bài đăng thất bại." });
                }

                return Json(new { success = true, message = "Đã xóa bài đăng!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {ProductId}", id);
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }
    }

    // Request DTOs
    public class ApproveRequest
    {
        public string? Note { get; set; }
    }

    public class RejectRequest
    {
        public string Reason { get; set; } = string.Empty;
    }

    public class DeleteRequest
    {
        public string Reason { get; set; } = string.Empty;
    }
}
