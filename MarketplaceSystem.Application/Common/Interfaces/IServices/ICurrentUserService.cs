namespace MarketplaceSystem.Application.Common.Interfaces.IServices
{
    public interface ICurrentUserService
    {
        int? UserId { get; }

        string? UserName { get; }

        bool IsAuthenticated { get; }

        IEnumerable<string> Roles { get; }

        string? IpAddress { get; }

        string? DeviceInfo { get; }
    }
}
