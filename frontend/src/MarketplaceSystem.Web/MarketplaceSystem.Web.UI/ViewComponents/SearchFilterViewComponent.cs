using MarketplaceSystem.Web.UI.Models.ViewModels.Pages;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceSystem.Web.UI.ViewComponents
{
    public class SearchFilterViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(HomePageViewModel model)
        {
            return View(model);
        }
    }
}
