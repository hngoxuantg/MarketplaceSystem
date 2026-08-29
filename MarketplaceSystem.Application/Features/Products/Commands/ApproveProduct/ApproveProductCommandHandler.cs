using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Entities.Identity_Auth;
using MarketplaceSystem.Domain.Enums.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;

namespace MarketplaceSystem.Application.Features.Products.Commands.ApproveProduct
{
    public class ApproveProductCommandHandler : IRequestHandler<ApproveProductCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ApproveProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(ApproveProductCommand command, CancellationToken cancellationToken)
        {
            return await ApproveProductAsync(command.Id, cancellationToken);
        }
        private async Task<bool> ApproveProductAsync(int id, CancellationToken cancellation = default)
        {
            Product? product = await _unitOfWork.ProductRepository.GetOneAsync<Product>(
                filter: p => p.Id == id && !p.IsDeleted,
                cancellation: cancellation
            ) ?? throw new NotFoundException("Sản phẩm không tồn tại");

            await ValidateForApproveOrRejectProductAsync(product, cancellation);

            product.Approve();
            await _unitOfWork.ProductRepository.UpdateAsync(product, cancellation);

            return true;
        }
        private async Task ValidateForApproveOrRejectProductAsync(Product product, CancellationToken cancellation = default)
        {
            if (product.Status != ProductStatus.PendingApproval && product.Status != ProductStatus.Rejected)
            {
                throw new BusinessRuleException("Chỉ có thể phê duyệt sản phẩm ở trạng thái chờ phê duyệt hoặc đã bị từ chối");
            }

            await IsValidSellerAsync(product.SellerId ?? 0, cancellation);
        }
        private async Task IsValidSellerAsync(int sellerId, CancellationToken cancellation = default)
        {
            User user = await _unitOfWork.UserRepository.GetOneUntrackedAsync(
                filter: u => u.Id == sellerId,
                cancellation: cancellation,
                selector: u => new User
                {
                    IsDeleted = u.IsDeleted,
                    Profile = new UserProfile
                    {
                        SellerVerificationStatus = u.Profile.SellerVerificationStatus
                    }
                }) ?? throw new NotFoundException("Người bán không tồn tại");

            if (user.IsDeleted)
            {
                throw new BusinessRuleException("Người bán đã bị xóa!");
            }
            if (user.Profile?.SellerVerificationStatus != SellerVerificationStatus.Approved)
            {
                throw new BusinessRuleException("Người bán chưa được xác minh!");
            }
        }
    }
}
