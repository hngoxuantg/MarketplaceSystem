using MarketplaceSystem.Common.Enums;
using MarketplaceSystem.Domain.Enums.Business;

namespace MarketplaceSystem.Application.Features.Auth.Commands.Register
{
    public class RegisterRequest
    {
        public string FullName { get; set; }
        public string Email { get; set; }

        public VietnamProvince Location { get; set; }
        public DateTime DateOfBirth { get; set; }
        public UserProfileGender Gender { get; set; }

        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
