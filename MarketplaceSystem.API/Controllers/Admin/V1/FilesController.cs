using MarketplaceSystem.Application.Features.Categories.Commands.UploadCategoryIcon;
using MarketplaceSystem.Common.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceSystem.API.Controllers.Admin.V1
{
    public class FilesController : BaseController
    {
        private readonly ISender _sender;

        public FilesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("upload-category-icon/{id}")]
        public async Task<IActionResult> UploadCategoryIconAsync(int id, IFormFile icon, CancellationToken cancellation = default)
        {
            var result = await _sender.Send(new UploadCategoryIconCommand(id, icon), cancellation);

            var response = new ApiResponse<string>
            {
                Success = true,
                Message = "Upload icon cho danh mục thành công!",
                Data = result
            };

            return Ok(response);
        }
    }
}
