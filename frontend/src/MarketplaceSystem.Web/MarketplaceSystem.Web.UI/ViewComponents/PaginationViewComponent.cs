using MarketplaceSystem.Web.UI.Models.ViewModels.Product;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceSystem.Web.UI.ViewComponents
{
    public class PaginationViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ProductListViewModel model)
        {
            if (model == null || model.TotalPages <= 1)
            {
                return Content(string.Empty);
            }

            return View(model);
        }
    }
}
