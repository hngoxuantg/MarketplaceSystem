using Microsoft.AspNetCore.Mvc;

namespace MarketplaceSystem.Web.UI.Admin.Controllers
{
    public class OrderController : Controller
    {
        private readonly ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        // GET: Order
        public IActionResult Index()
        {
            return View();
        }

        // GET: Order/Details/5
        public IActionResult Details(int id)
        {
            return View();
        }
    }
}
