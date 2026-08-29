using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;

namespace MarketplaceSystem.Application.Features.Products.Commands.DeleteProductByAdmin
{
    public class DeleteProductByAdminCommandHandler : IRequestHandler<DeleteProductByAdminCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteProductByAdminCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(DeleteProductByAdminCommand command, CancellationToken cancellationToken)
        {
            return await DeleteProductByAdminAsync(command.Id, cancellationToken);
        }
        private async Task<bool> DeleteProductByAdminAsync(int id, CancellationToken cancellation = default)
        {
            Product product = await _unitOfWork.ProductRepository.GetOneAsync<Product>(
                filter: p => p.Id == id && !p.IsDeleted,
                cancellation: cancellation
            ) ?? throw new NotFoundException("Sản phẩm không tồn tại");

            await _unitOfWork.ProductRepository.DeleteAsync(product, cancellation);

            return true;
        }
    }
}
