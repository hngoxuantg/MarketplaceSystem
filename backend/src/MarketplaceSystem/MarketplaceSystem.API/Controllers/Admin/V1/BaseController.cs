using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceSystem.API.Controllers.Admin.V1
{
    [ApiVersion("1.0")]
    [Route("api/admin/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [ApiExplorerSettings(GroupName = "admin-v1")]
    public abstract class BaseController : ControllerBase
    {
        protected virtual string RefreshToken => Request.Cookies["refreshToken"] ?? string.Empty;
    }
}
