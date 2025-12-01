using MarketplaceSystem.Web.UI.Interfaces;
using MarketplaceSystem.Web.UI.Models.ViewModels.Pages;
using MarketplaceSystem.Web.UI.Models.ViewModels.Product;
using MarketplaceSystem.Web.UI.Models.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceSystem.Web.UI.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IEnumService _enumService;
        public HomeController(IProductService productService, ICategoryService categoryService, IEnumService enumService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _enumService = enumService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(
            string? Search,
            int? CategoryId,
            int? Location,
            int PageNumber = 1,
            int PageSize = 15,
            CancellationToken cancellation = default)
        {
            var model = new HomePageViewModel();

            var categoriesResponse = await _categoryService.GetHomePageCategoriesAsync(cancellation);
            model.RootCategories = categoriesResponse?.Data;

            var locationsResponse = await _enumService.GetVietnamProvincesAsync(cancellation);
            if (locationsResponse?.Data != null)
            {
                model.Locations = locationsResponse.Data.Select(l => new LocationItem
                {
                    Id = l.Id,
                    Name = l.Name
                }).ToList();
            }

            var filter = new ProductFilterViewModel
            {
                Search = Search,
                CategoryId = CategoryId,
                Location = Location,
                PageNumber = PageNumber,
                PageSize = PageSize
            };

            var productsResponse = await _productService.GetProductsAsync(filter, cancellation);
            if (productsResponse?.Data != null)
            {
                model.Products = new ProductListViewModel
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
        [HttpPost]
        public async Task<IActionResult> Search(ProductFilterViewModel filter, CancellationToken cancellation = default)
        {
            var response = await _productService.GetProductsAsync(filter, cancellation);

            var products = response?.Data != null ? new ProductListViewModel
            {
                Items = response.Data.Data,
                TotalCount = response.Data.TotalCount,
                PageNumber = response.Data.PageNumber,
                PageSize = response.Data.PageSize,
                TotalPages = response.Data.TotalPages,
                HasPreviousPage = response.Data.HasPreviousPage,
                HasNextPage = response.Data.HasNextPage
            } : new ProductListViewModel();

            return PartialView("_ProductListPartial", products);
        }
        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Terms()
        {
            return View();
        }

        [HttpGet]
        public IActionResult About()
        {
            return View();
        }
    }
}
