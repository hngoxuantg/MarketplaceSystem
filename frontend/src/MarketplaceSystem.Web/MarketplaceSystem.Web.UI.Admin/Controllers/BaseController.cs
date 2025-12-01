using Microsoft.AspNetCore.Mvc;

namespace MarketplaceSystem.Web.UI.Admin.Controllers
{
    public class BaseController : Controller
    {
        public IActionResult Error()
        {
            return View("Error");
        }
        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }
        public IActionResult NotFoundPage()
        {
            return View("NotFound");
        }
        public IActionResult ServerError()
        {
            return View("ServerError");
        }
        public IActionResult EnsureAuthenticated()
        {
            return View();
        }
        public bool IsAuthenticated()
        {
            return User.Identity != null && User.Identity.IsAuthenticated;
        }
        protected virtual void MapApiErrorsToModelState(Dictionary<string, string[]> errors)
        {
            if (errors == null || !errors.Any())
                return;

            foreach (var error in errors)
            {
                var fieldName = error.Key;
                foreach (var errorMessage in error.Value)
                {
                    ModelState.AddModelError(fieldName, errorMessage);
                }
            }
        }
    }
}
