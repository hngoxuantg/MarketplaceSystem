using MarketplaceSystem.Web.UI.Admin.Interfaces;
using MarketplaceSystem.Web.UI.Admin.Models.ViewModel.Users;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceSystem.Web.UI.Admin.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Trang danh sách người dùng
        /// GET: /User/Index
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 12, string? search = null, int? status = null)
        {
            try
            {
                if (!IsAuthenticated())
                {
                    return RedirectToAction("Login", "Auth");
                }

                var users = await _userService.GetUsersAsync(pageNumber, pageSize, search, status);
                
                ViewBag.CurrentPage = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.Search = search;
                ViewBag.Status = status;

                return View(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách người dùng");
                TempData["Error"] = "Không thể tải danh sách người dùng. Vui lòng thử lại sau.";
                return View();
            }
        }

        /// <summary>
        /// Trang chi tiết người dùng
        /// GET: /User/Details/{id}
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                if (!IsAuthenticated())
                {
                    return RedirectToAction("Login", "Auth");
                }

                var user = await _userService.GetUserByIdAsync(id);
                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin người dùng {UserId}", id);
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// API khóa người dùng
        /// POST: /User/Lock
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Lock([FromBody] LockUserModel model)
        {
            try
            {
                if (!IsAuthenticated())
                {
                    return Unauthorized(new { success = false, message = "Vui lòng đăng nhập" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                var request = new LockUserRequest
                {
                    Until = model.Until
                };

                await _userService.LockUserAsync(model.Id, request);

                return Ok(new { success = true, message = "Khóa người dùng thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi khóa người dùng {UserId}", model.Id);
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// API mở khóa người dùng
        /// POST: /User/Unlock
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Unlock([FromBody] UnlockUserModel model)
        {
            try
            {
                if (!IsAuthenticated())
                {
                    return Unauthorized(new { success = false, message = "Vui lòng đăng nhập" });
                }

                await _userService.UnlockUserAsync(model.Id);

                return Ok(new { success = true, message = "Mở khóa người dùng thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi mở khóa người dùng {UserId}", model.Id);
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // GET: User/Posts/5 - Bài đăng của user
        public IActionResult Posts(int id)
        {
            // TODO: Lấy danh sách bài đăng của user
            // var posts = await _userService.GetUserPostsAsync(id);
            ViewBag.UserId = id;
            return View();
        }
    }

    // Models cho request từ client
    public class LockUserModel
    {
        public int Id { get; set; }
        public DateTime Until { get; set; }
    }

    public class UnlockUserModel
    {
        public int Id { get; set; }
    }
}
