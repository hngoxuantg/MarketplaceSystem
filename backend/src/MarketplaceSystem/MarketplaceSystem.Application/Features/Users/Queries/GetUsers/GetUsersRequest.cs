using MarketplaceSystem.Common.Models.Pagination;

namespace MarketplaceSystem.Application.Features.Users.Queries.GetUsers
{
    public class GetUsersRequest : PaginatedRequest
    {
        public UserStatus? Status { get; set; }
    }
    public enum UserStatus
    {
        Active = 1,
        Locked = 2,
        PendingEmailConfirmation = 3
    }
}
