using System.Net;

namespace MarketplaceSystem.Application.Common.Exceptions
{
    public class UnauthorizedAccessException : BaseCustomException
    {
        public override HttpStatusCode HttpStatusCode => HttpStatusCode.Unauthorized;
        public override string ErrorCode => "UNAUTHORIZED_ACCESS";

        public UnauthorizedAccessException() { }
        public UnauthorizedAccessException(string? message) : base(message) { }
        public UnauthorizedAccessException(string message, Exception? innerException) : base(message, innerException) { }
    }
}
