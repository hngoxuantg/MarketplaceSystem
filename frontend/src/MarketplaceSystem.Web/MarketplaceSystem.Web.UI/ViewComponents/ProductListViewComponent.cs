using MarketplaceSystem.Web.UI.Models.ViewModels.Product;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceSystem.Web.UI.ViewComponents
{
    public class ProductListViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ProductListViewModel model)
        {
            if (model == null || !model.Items.Any())
            {
                return View("Empty");
            }

            return View(model);
        }
    }
}
