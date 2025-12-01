using MarketplaceSystem.Web.UI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceSystem.Web.UI.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<IActionResult> Detail(int id, CancellationToken cancellation = default)
        {
            var result = await _productService.GetProductByIdAysnc(id, cancellation);

            if (result?.Data != null)
            {
                return View(result.Data);
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetProductJson(int id, CancellationToken cancellation = default)
        {
            try
            {
                var result = await _productService.GetProductByIdAysnc(id, cancellation);

                if (result?.Data != null)
                {
                    return Json(new { success = true, data = result.Data });
                }

                return Json(new { success = false, data = (object)null });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message, data = (object)null });
            }
        }
    }
}
