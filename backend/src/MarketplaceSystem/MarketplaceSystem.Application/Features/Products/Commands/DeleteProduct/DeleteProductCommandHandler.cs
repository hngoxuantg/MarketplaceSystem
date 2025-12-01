using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Application.Common.Interfaces.IServices;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;

namespace MarketplaceSystem.Application.Features.Products.Commands.DeleteProduct
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public DeleteProductCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }
        public async Task<bool> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
        {
            return await DeleteProductAsync(command.Id, cancellationToken);
        }
        private async Task<bool> DeleteProductAsync(int id, CancellationToken cancellation = default)
        {
            Product product = await _unitOfWork.ProductRepository.GetOneAsync<Product>(
                filter: p => p.Id == id && !p.IsDeleted && p.SellerId == _currentUserService.UserId,
                cancellation: cancellation
            ) ?? throw new NotFoundException("Sản phẩm không tồn tại");

            ValidateUserExists(product.SellerId!.Value);

            await _unitOfWork.ProductRepository.DeleteAsync(product, cancellation);

            return true;
        }
        private void ValidateUserExists(int userId)
        {
            if (userId != _currentUserService.UserId)
            {
                throw new ForbiddenAccessException("Bạn không được phép truy cập vào tài nguyên này!");
            }
        }
    }
}
