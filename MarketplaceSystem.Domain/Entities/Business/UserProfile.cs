using MarketplaceSystem.Common.Enums;
using MarketplaceSystem.Domain.Entities.Base;
using MarketplaceSystem.Domain.Entities.Identity_Auth;
using MarketplaceSystem.Domain.Enums.Business;

namespace MarketplaceSystem.Domain.Entities.Business
{
    public class UserProfile : SoftDeleteEntity
    {
        public string FullName { get; set; }
        public string? Avatar { get; set; }
        public string? Bio { get; set; }
        public string? ShowPhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public UserProfileGender Gender { get; set; }
        public VietnamProvince Location { get; set; }
        public SellerVerificationStatus SellerVerificationStatus { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
