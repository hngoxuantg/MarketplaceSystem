using MarketplaceSystem.Web.UI.Models.ApiResponses;
using MarketplaceSystem.Web.UI.Models.ViewModels.Category;
using MarketplaceSystem.Web.UI.Models.ViewModels.Post;

namespace MarketplaceSystem.Web.UI.Models.ViewModels.Pages
{
    public class PostPageViewModel
    {
        public List<CategoryAttributeViewModel> Attributes { get; set; }

        public List<EnumsResponse>? Location { get; set; }

        public List<EnumsResponse>? Condition { get; set; }

        public CreatePostRequest Request { get; set; } = new CreatePostRequest();

        public PostPageViewModel()
        {
            Attributes = new List<CategoryAttributeViewModel>();
        }
    }

    public class CreatePostRequest
    {
        public CreatePostViewModel Post { get; set; } = new CreatePostViewModel();
        public UploadImageViewModel ImageViewModel { get; set; } = new UploadImageViewModel();
    }
    public class UploadImageViewModel
    {
        public int ProductId { get; set; }

        public List<IFormFile> Images { get; set; }

        public int IsMainIndex { get; set; }
    }
}
