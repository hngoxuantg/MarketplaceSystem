using MarketplaceSystem.Web.UI.Interfaces;
using MarketplaceSystem.Web.UI.Interfaces.IBaseServices;
using MarketplaceSystem.Web.UI.Models.ApiResponses.Common;
using MarketplaceSystem.Web.UI.Models.ViewModels.Category;
using MarketplaceSystem.Web.UI.Models.ViewModels.Pages;
using MarketplaceSystem.Web.UI.Models.ViewModels.Product;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MarketplaceSystem.Web.UI.Controllers
{
    public class PostController : BaseController
    {
        private readonly IBaseApiService _baseApiService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICategoryService _categoryService;
        private readonly IEnumService _enumService;
        private readonly IProductService _productService;

        public PostController(
            IBaseApiService baseApiService,
            IHttpClientFactory httpClientFactory,
            ICategoryService categoryService,
            IEnumService enumService,
            IProductService productService)
        {
            _baseApiService = baseApiService;
            _httpClientFactory = httpClientFactory;
            _categoryService = categoryService;
            _enumService = enumService;
            _productService = productService;
        }
        public async Task<IActionResult> SelectCategory(CancellationToken cancellation = default)
        {
            ApiResponse<List<ListingCategoryViewModel>>? response = await _categoryService.GetListingCategoriesAsync(cancellation);

            return View(response.Data);
        }

        public async Task<IActionResult> Create(int categoryId, CancellationToken cancellation = default)
        {
            PostPageViewModel vm = new PostPageViewModel();
            vm.Request.Post.CategoryId = categoryId;

            vm.Location = (await _enumService.GetVietnamProvincesAsync(cancellation)).Data;
            vm.Condition = (await _enumService.GetProductConditionsAsync(cancellation)).Data;
            vm.Attributes = (await _categoryService.GetCategoryAttributesAsync(categoryId, cancellation)).Data;

            return View(vm);
        }

        private async Task<PostPageViewModel> BuildPostPageViewModel(PostPageViewModel model, CancellationToken cancellation)
        {
            var vm = new PostPageViewModel
            {
                Request = model.Request,
                Location = (await _enumService.GetVietnamProvincesAsync(cancellation)).Data,
                Condition = (await _enumService.GetProductConditionsAsync(cancellation)).Data,
                Attributes = (await _categoryService.GetCategoryAttributesAsync(model.Request.Post.CategoryId, cancellation)).Data
            };
            return vm;
        }

        [HttpPost]
        public async Task<IActionResult> Create(PostPageViewModel model, CancellationToken cancellation = default)
        {
            var result = await _productService.CreateProductAsync(model.Request, cancellation);

            if (result.Success)
            {
                TempData["Success"] = "Đăng tin thành công!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Đăng tin thất bại. Vui lòng thử lại.");
                if (result.Errors != null)
                {
                    MapApiErrorsToModelState(result.Errors);
                }

                return View(await BuildPostPageViewModel(model, cancellation));
            }
        }
        protected override void MapApiErrorsToModelState(Dictionary<string, string[]> errors)
        {
            if (errors == null || !errors.Any()) return;

            foreach (var error in errors)
            {
                var originalKey = error.Key.Trim();
                var lower = originalKey.ToLowerInvariant();

                string mappedField;

                if (Regex.IsMatch(lower, @"^attributevalues\[\d+\]\.", RegexOptions.IgnoreCase))
                {
                    var m = Regex.Match(originalKey, @"^(attributevalues)(\[\d+\])\.(.+)$", RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        var propAfter = m.Groups[3].Value;
                        var pascalAfter = ToPascalCase(propAfter);
                        mappedField = $"Request.Post.AttributeValues{m.Groups[2].Value}.{pascalAfter}";
                    }
                    else
                    {
                        mappedField = $"Request.Post.{originalKey}";
                    }
                }
                else
                {
                    mappedField = lower switch
                    {
                        "title" => "Request.Post.Title",
                        "description" => "Request.Post.Description",
                        "price" => "Request.Post.Price",
                        "condition" => "Request.Post.Condition",
                        "location" => "Request.Post.Location",
                        "images" => "Request.ImageViewModel.Images",
                        "ismainindex" => "Request.ImageViewModel.IsMainIndex",
                        _ => $"Request.Post.{ToPascalCase(originalKey)}"
                    };
                }

                foreach (var msg in error.Value)
                    ModelState.AddModelError(mappedField, msg);
            }
        }

        private static string ToPascalCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            var parts = input.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            var last = parts.Last();
            last = Regex.Replace(last, @"[_\-]", " ");
            TextInfo ti = CultureInfo.InvariantCulture.TextInfo;
            last = ti.ToTitleCase(last.ToLowerInvariant()).Replace(" ", "");
            return last;
        }

        // GET: /Post/MyPosts - Xem tin đã đăng
        public async Task<IActionResult> MyPosts(int PageNumber = 1, int PageSize = 20, CancellationToken cancellation = default)
        {
            var filter = new ProductFilterViewModel
            {
                PageNumber = PageNumber,
                PageSize = PageSize
                // Backend API sẽ tự động filter theo UserId từ JWT token
            };

            var productsResponse = await _productService.GetProductsAsync(filter, cancellation);

            var model = new ProductListViewModel();
            if (productsResponse?.Data != null)
            {
                model = new ProductListViewModel
                { 
                    Items = productsResponse.Data.Data,
                    TotalCount = productsResponse.Data.TotalCount,
                    PageNumber = productsResponse.Data.PageNumber,
                    PageSize = productsResponse.Data.PageSize,
                    TotalPages = productsResponse.Data.TotalPages,
                    HasPreviousPage = productsResponse.Data.HasPreviousPage,
                    HasNextPage = productsResponse.Data.HasNextPage
                };
            }

            return View(model);
        }

        // GET: /Post/FavoritePosts - Xem tin đã thích
        public async Task<IActionResult> FavoritePosts(CancellationToken cancellation = default)
        {
            // View sẽ load favorites từ localStorage và call API để lấy chi tiết
            // Truyền service để view có thể dùng
            ViewBag.ProductService = _productService;
            return View();
        }

        // API: Get favorites from backend
        [HttpGet]
        public async Task<IActionResult> GetFavoritesFromApi(CancellationToken cancellation = default)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null)
                {
                    // Nếu chưa login, dùng localStorage
                    return Json(new { success = false, message = "Not logged in", useLocalStorage = true });
                }

                // Call backend API: GET /api/user/v1/users/{userId}/favorites
                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync($"https://localhost:7079/api/user/v1/users/{userId}/favorites", cancellation);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync(cancellation);
                    return Content(content, "application/json");
                }

                return Json(new { success = false, message = "Failed to load favorites" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // POST: /Post/MarkAsSold - Đánh dấu đã bán
        [HttpPost]
        public async Task<IActionResult> MarkAsSold(int id, CancellationToken cancellation = default)
        {
            try
            {
                var result = await _productService.MarkAsSoldAsync(id, cancellation);

                if (result?.Success == true)
                {
                    return Json(new { success = true, message = result.Message ?? "Đã đánh dấu sản phẩm là đã bán" });
                }

                return Json(new { success = false, message = result?.Message ?? "Không thể đánh dấu sản phẩm" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // POST: /Post/Delete - Xóa tin đăng
        [HttpPost]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellation = default)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id, cancellation);

                if (result?.Success == true)
                {
                    return Json(new { success = true, message = result.Message ?? "Đã xóa tin đăng thành công" });
                }

                return Json(new { success = false, message = result?.Message ?? "Không thể xóa tin đăng" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }
    }
}