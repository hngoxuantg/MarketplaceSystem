using MarketplaceSystem.Web.UI.Admin.Interfaces;
using MarketplaceSystem.Web.UI.Admin.Models.ViewModel.Categories;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceSystem.Web.UI.Admin.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly ICategoryService _categoryService;

        public CategoryController(ILogger<CategoryController> logger, ICategoryService categoryService)
        {
            _logger = logger;
            _categoryService = categoryService;
        }

        // GET: Category - Danh sách danh mục
        public async Task<IActionResult> Index(
            int pageNumber = 1,
            int pageSize = 10,
            string? searchTerm = null,
            bool? isActive = null,
            CancellationToken cancellation = default)
        {
            var result = await _categoryService.GetCategoriesAsync(pageNumber, pageSize, searchTerm, isActive, cancellation);

            if (result?.Data == null)
            {
                return View(new CategoryIndexViewModel());
            }

            // Lấy statistics từ tất cả categories (không phân trang) nếu cần
            // Hoặc backend nên trả về statistics riêng
            var viewModel = new CategoryIndexViewModel
            {
                Categories = result.Data,
                TotalCategories = result.Data.TotalCount, // Tổng số từ backend
                ActiveCategories = result.Data.Data.Count(c => c.IsActive), // Tạm tính từ trang hiện tại
                InactiveCategories = result.Data.Data.Count(c => !c.IsActive), // Tạm tính từ trang hiện tại
                TotalPosts = result.Data.Data.Sum(c => c.SubCategories?.Count ?? 0), // Tạm tính từ trang hiện tại
                SearchTerm = searchTerm,
                IsActive = isActive
            };

            return View(viewModel);
        }

        // GET: Category/GetCategoriesPartial - Load partial view
        public async Task<IActionResult> GetCategoriesPartial(
            int pageNumber = 1,
            int pageSize = 10,
            string? searchTerm = null,
            bool? isActive = null,
            CancellationToken cancellation = default)
        {
            var result = await _categoryService.GetCategoriesAsync(pageNumber, pageSize, searchTerm, isActive, cancellation);

            if (result?.Data == null)
            {
                return PartialView("_CategoriesTablePartial", new CategoryIndexViewModel());
            }

            var viewModel = new CategoryIndexViewModel
            {
                Categories = result.Data,
                TotalCategories = result.Data.TotalCount,
                ActiveCategories = result.Data.Data.Count(c => c.IsActive),
                InactiveCategories = result.Data.Data.Count(c => !c.IsActive),
                TotalPosts = result.Data.Data.Sum(c => c.SubCategories?.Count ?? 0),
                SearchTerm = searchTerm,
                IsActive = isActive
            };

            return PartialView("_CategoriesTablePartial", viewModel);
        }

        // GET: Category/Details/{id} - Xem chi tiết danh mục
        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken cancellation = default)
        {
            try
            {
                var result = await _categoryService.GetCategoryByIdAsync(id, cancellation);

                if (result == null || !result.Success || result.Data == null)
                {
                    TempData["Error"] = result?.Message ?? "Không tìm thấy danh mục!";
                    return RedirectToAction(nameof(Index));
                }

                return View(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category details");
                TempData["Error"] = "Có lỗi xảy ra khi tải thông tin danh mục.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Category/Edit/{id} - Form chỉnh sửa danh mục
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellation = default)
        {
            try
            {
                var result = await _categoryService.GetCategoryByIdAsync(id, cancellation);

                if (result == null || !result.Success || result.Data == null)
                {
                    TempData["Error"] = result?.Message ?? "Không tìm thấy danh mục!";
                    return RedirectToAction(nameof(Index));
                }

                var category = result.Data;

                // Get all parent categories for dropdown
                var allCategoriesResult = await _categoryService.GetCategoriesAsync(1, 100, null, null, cancellation);
                var parentCategories = allCategoriesResult?.Data?.Data
                    .Where(c => c.IsParentCategory && c.Id != id)
                    .ToList() ?? new List<CategoryViewModel>();

                ViewBag.ParentCategories = parentCategories;

                return View(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading edit form");
                TempData["Error"] = "Có lỗi xảy ra khi tải form chỉnh sửa.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Category/Edit/{id} - Cập nhật danh mục (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCategoryAjax(int id, [FromBody] UpdateCategoryViewModel model, CancellationToken cancellation = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ: " + string.Join(", ", errors) });
                }

                // Call API to update category
                var updateResult = await _categoryService.UpdateCategoryAsync(id, model, cancellation);

                if (updateResult == null || !updateResult.Success)
                {
                    return Json(new { success = false, message = updateResult?.Message ?? "Cập nhật danh mục thất bại." });
                }

                return Json(new { success = true, message = "Cập nhật danh mục thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category");
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật danh mục: " + ex.Message });
            }
        }

        // POST: Category/Edit/{id} - Cập nhật danh mục (Form submission - keep for backward compatibility)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryViewModel model, CancellationToken cancellation = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var result = await _categoryService.GetCategoriesAsync(cancellation);
                    ViewBag.ParentCategories = result?.Data?.Data
                        .Where(c => c.IsParentCategory && c.Id != id)
                        .ToList() ?? new List<CategoryViewModel>();
                    return View(model);
                }

                // Map to UpdateCategoryViewModel
                var updateModel = new UpdateCategoryViewModel
                {
                    Name = model.Name,
                    Description = model.Description,
                    ParentCategoryId = model.ParentCategoryId,
                    DisplayOrder = model.DisplayOrder,
                    IsActive = model.IsActive
                };

                // Call API to update category
                var updateResult = await _categoryService.UpdateCategoryAsync(id, updateModel, cancellation);

                if (updateResult == null || !updateResult.Success)
                {
                    TempData["Error"] = updateResult?.Message ?? "Cập nhật danh mục thất bại.";
                    if (updateResult?.Errors != null)
                    {
                        MapApiErrorsToModelState(updateResult.Errors);
                    }

                    var result = await _categoryService.GetCategoriesAsync(cancellation);
                    ViewBag.ParentCategories = result?.Data?.Data
                        .Where(c => c.IsParentCategory && c.Id != id)
                        .ToList() ?? new List<CategoryViewModel>();
                    return View(model);
                }

                TempData["Success"] = "Cập nhật danh mục thành công!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category");
                TempData["Error"] = "Có lỗi xảy ra khi cập nhật danh mục.";
                return View(model);
            }
        }

        // GET: Category/Create - Form tạo danh mục mới
        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellation = default)
        {
            try
            {
                var result = await _categoryService.GetCategoriesAsync(cancellation);
                ViewBag.ParentCategories = result?.Data?.Data
                    .Where(c => c.IsParentCategory)
                    .ToList() ?? new List<CategoryViewModel>();

                return View(new CreateCategoryViewModel { Name = "" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create form");
                TempData["Error"] = "Có lỗi xảy ra khi tải form tạo mới.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Category/Create - Tạo danh mục mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryViewModel model, CancellationToken cancellation = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var result = await _categoryService.GetCategoriesAsync(cancellation);
                    ViewBag.ParentCategories = result?.Data?.Data
                        .Where(c => c.IsParentCategory)
                        .ToList() ?? new List<CategoryViewModel>();
                    return View(model);
                }

                // Create category
                var createResult = await _categoryService.CreateCategoryAsync(model, cancellation);

                if (createResult == null || !createResult.Success)
                {
                    ViewBag.Error = createResult?.Message ?? "Tạo danh mục thất bại. Vui lòng thử lại.";
                    if (createResult?.Errors != null)
                    {
                        MapApiErrorsToModelState(createResult.Errors);
                    }

                    var resultCategories = await _categoryService.GetCategoriesAsync(cancellation);
                    ViewBag.ParentCategories = resultCategories?.Data?.Data
                        .Where(c => c.IsParentCategory)
                        .ToList() ?? new List<CategoryViewModel>();
                    return View(model);
                }

                var createdCategory = createResult.Data;
                if (createdCategory == null)
                {
                    TempData["Error"] = "Không thể lấy thông tin danh mục vừa tạo.";
                    return RedirectToAction(nameof(Index));
                }

                // Upload icon if provided
                if (model.IconFile != null && model.IconFile.Length > 0)
                {
                    try
                    {
                        using var stream = model.IconFile.OpenReadStream();
                        var uploadResult = await _categoryService.UploadCategoryIconAsync(
                            createdCategory.Id,
                            stream,
                            model.IconFile.FileName,
                            cancellation
                        );

                        if (uploadResult == null || !uploadResult.Success)
                        {
                            _logger.LogWarning("Failed to upload icon for category {CategoryId}: {Message}",
                                createdCategory.Id, uploadResult?.Message);
                            // Không fail toàn bộ, chỉ warning
                            TempData["Warning"] = "Danh mục đã được tạo nhưng không thể upload icon.";
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error uploading icon for category {CategoryId}", createdCategory.Id);
                        TempData["Warning"] = "Danh mục đã được tạo nhưng không thể upload icon.";
                    }
                }

                TempData["Success"] = "Tạo danh mục mới thành công!";
                return RedirectToAction(nameof(Details), new { id = createdCategory.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                TempData["Error"] = "Có lỗi xảy ra khi tạo danh mục.";

                var resultCategories = await _categoryService.GetCategoriesAsync(cancellation);
                ViewBag.ParentCategories = resultCategories?.Data?.Data
                    .Where(c => c.IsParentCategory)
                    .ToList() ?? new List<CategoryViewModel>();
                return View(model);
            }
        }

        // POST: Category/ToggleStatus - Bật/tắt danh mục
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, CancellationToken cancellation = default)
        {
            try
            {
                // TODO: Gọi API để bật/tắt danh mục
                // await _categoryService.ToggleStatusAsync(id, cancellation);
                return Json(new { success = true, message = "Đã cập nhật trạng thái danh mục!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling category status");
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // POST: Category/Delete - Xóa danh mục (Form submission - từ Details page)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellation = default)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id, cancellation);

                if (result == null || !result.Success)
                {
                    TempData["Error"] = result?.Message ?? "Xóa danh mục thất bại.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                TempData["Success"] = "Đã xóa danh mục thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category");
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // POST: Category/DeleteAjax - Xóa danh mục (AJAX - từ Index page)
        [HttpPost]
        public async Task<IActionResult> DeleteAjax([FromBody] int id, CancellationToken cancellation = default)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id, cancellation);

                if (result == null || !result.Success)
                {
                    return Json(new { success = false, message = result?.Message ?? "Xóa danh mục thất bại." });
                }

                return Json(new { success = true, message = "Đã xóa danh mục thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category");
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // POST: Category/ToggleActive - Bật/tắt trạng thái danh mục (AJAX)
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> ToggleActive([FromBody] int id, CancellationToken cancellation = default)
        {
            try
            {
                var result = await _categoryService.ToggleActiveAsync(id, cancellation);

                if (result == null || !result.Success)
                {
                    return Json(new { success = false, message = result?.Message ?? "Cập nhật trạng thái thất bại." });
                }

                return Json(new { success = true, message = "Đã cập nhật trạng thái danh mục!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling category active status");
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // GET: Category/GetDetails - Get category details JSON (for AJAX)
        [HttpGet]
        public async Task<IActionResult> GetDetails(int id, CancellationToken cancellation = default)
        {
            try
            {
                var result = await _categoryService.GetCategoryByIdAsync(id, cancellation);

                if (result == null || !result.Success || result.Data == null)
                {
                    return Json(new { success = false, message = result?.Message ?? "Không tìm thấy danh mục!" });
                }

                return Json(new { success = true, data = result.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category details");
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // POST: Category/UpdateAttribute - Cập nhật thuộc tính
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAttribute(int categoryId, int attributeId, UpdateCategoryAttributeViewModel model, CancellationToken cancellation = default)
        {
            try
            {
                var result = await _categoryService.UpdateAttributeAsync(categoryId, attributeId, model, cancellation);

                if (result == null || !result.Success)
                {
                    TempData["Error"] = result?.Message ?? "Cập nhật thuộc tính thất bại.";
                }
                else
                {
                    TempData["Success"] = "Cập nhật thuộc tính thành công!";
                }

                return RedirectToAction(nameof(Edit), new { id = categoryId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating attribute");
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction(nameof(Edit), new { id = categoryId });
            }
        }

        // POST: Category/CreateAttribute - Tạo thuộc tính mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAttribute(int categoryId, CreateAttributeForCategoryViewModel model, CancellationToken cancellation = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    TempData["Error"] = "Dữ liệu không hợp lệ: " + string.Join(", ", errors);
                    return RedirectToAction(nameof(Edit), new { id = categoryId });
                }

                var result = await _categoryService.CreateAttributeAsync(categoryId, model, cancellation);

                if (result == null || !result.Success)
                {
                    TempData["Error"] = result?.Message ?? "Tạo thuộc tính thất bại.";
                }
                else
                {
                    TempData["Success"] = "Tạo thuộc tính thành công!";
                }

                return RedirectToAction(nameof(Edit), new { id = categoryId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating attribute");
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction(nameof(Edit), new { id = categoryId });
            }
        }

        // POST: Category/DeleteAttribute - Xóa thuộc tính
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAttribute(int categoryId, int attributeId, CancellationToken cancellation = default)
        {
            try
            {
                var result = await _categoryService.DeleteAttributeAsync(categoryId, attributeId, cancellation);

                if (result == null || !result.Success)
                {
                    TempData["Error"] = result?.Message ?? "Xóa thuộc tính thất bại.";
                }
                else
                {
                    TempData["Success"] = "Đã xóa thuộc tính!";
                }

                return RedirectToAction(nameof(Edit), new { id = categoryId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting attribute");
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction(nameof(Edit), new { id = categoryId });
            }
        }

        // POST: Category/UpdateAttributeOption - Cập nhật option
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAttributeOption(int categoryId, int attributeId, int optionId, UpdateAttributeOptionViewModel model, CancellationToken cancellation = default)
        {
            try
            {
                var result = await _categoryService.UpdateAttributeOptionAsync(categoryId, attributeId, optionId, model, cancellation);

                if (result == null || !result.Success)
                {
                    TempData["Error"] = result?.Message ?? "Cập nhật option thất bại.";
                }
                else
                {
                    TempData["Success"] = "Cập nhật option thành công!";
                }

                return RedirectToAction(nameof(Edit), new { id = categoryId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating attribute option");
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction(nameof(Edit), new { id = categoryId });
            }
        }

        // POST: Category/DeleteAttributeOption - Xóa option
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAttributeOption(int categoryId, int attributeId, int optionId, CancellationToken cancellation = default)
        {
            try
            {
                var result = await _categoryService.DeleteAttributeOptionAsync(categoryId, attributeId, optionId, cancellation);

                if (result == null || !result.Success)
                {
                    TempData["Error"] = result?.Message ?? "Xóa option thất bại.";
                }
                else
                {
                    TempData["Success"] = "Đã xóa option!";
                }

                return RedirectToAction(nameof(Edit), new { id = categoryId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting attribute option");
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction(nameof(Edit), new { id = categoryId });
            }
        }

        // ===== SIMPLE FORM-BASED ACTIONS (Non-AJAX) =====

        // POST: Category/UpdateCategory - Update category info (Form submission)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryViewModel model, CancellationToken cancellation = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Dữ liệu không hợp lệ!";
                    return RedirectToAction(nameof(Edit), new { id });
                }

                var result = await _categoryService.UpdateCategoryAsync(id, model, cancellation);

                if (result == null || !result.Success)
                {
                    TempData["Error"] = result?.Message ?? "Cập nhật danh mục thất bại.";
                }
                else
                {
                    TempData["Success"] = "Cập nhật danh mục thành công!";
                }

                return RedirectToAction(nameof(Edit), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category");
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction(nameof(Edit), new { id });
            }
        }
    }
}
