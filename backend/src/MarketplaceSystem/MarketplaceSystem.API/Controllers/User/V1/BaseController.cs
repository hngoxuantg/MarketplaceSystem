using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceSystem.API.Controllers.User.V1
{
    [ApiVersion("1.0")]
    [Route("api/user/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiExplorerSettings(GroupName = "user-v1")]
    public abstract class BaseController : ControllerBase
    {
    }
}
