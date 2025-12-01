using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Application.Common.Interfaces.IServices;
using MarketplaceSystem.Domain.Enums.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;

namespace MarketplaceSystem.Application.Features.Products.Commands.MarkAsSold
{
    public class MarkAsSoldCommandHandler : IRequestHandler<MarkAsSoldCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public MarkAsSoldCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(MarkAsSoldCommand command, CancellationToken cancellationToken = default)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(command.ProductId, cancellationToken)
                ?? throw new NotFoundException("Sản phẩm không tồn tại");

            if (product.SellerId != _currentUserService.UserId)
            {
                throw new ForbiddenAccessException("Bạn không có quyền thực hiện hành động này");
            }

            if (product.Status == ProductStatus.Sold)
            {
                throw new BusinessRuleException("Sản phẩm đã được đánh dấu là đã bán");
            }

            if (product.Status != ProductStatus.Active)
            {
                throw new BusinessRuleException("Chỉ có thể đánh dấu đã bán cho sản phẩm đang bán");
            }

            product.MarkAsSold();
            product.SetUpdated(_currentUserService.UserId);

            await _unitOfWork.ProductRepository.UpdateAsync(product, cancellationToken);

            return true;
        }
    }
}
