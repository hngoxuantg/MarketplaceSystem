using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceSystem.API.Controllers.Public.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [ApiExplorerSettings(GroupName = "public-v1")]
    public abstract class BaseController : ControllerBase
    {
        protected virtual string RefreshToken => Request.Cookies["refreshToken"] ?? string.Empty;
    }
}
