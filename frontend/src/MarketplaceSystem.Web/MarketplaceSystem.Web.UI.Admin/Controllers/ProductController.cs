using Microsoft.AspNetCore.Mvc;

namespace MarketplaceSystem.Web.UI.Admin.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;

        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }

        // GET: Product
        public IActionResult Index()
        {
            return View();
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            return View();
        }

        // GET: Product/Edit/5
        public IActionResult Edit(int id)
        {
            return View();
        }

        // GET: Product/Details/5
        public IActionResult Details(int id)
        {
            return View();
        }
    }
}
