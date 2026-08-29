using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;

namespace MarketplaceSystem.Application.Features.Products.Commands.IncrementViewCount
{
    public class IncrementViewCountCommandHandler : IRequestHandler<IncrementViewCountCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public IncrementViewCountCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(IncrementViewCountCommand command, CancellationToken cancellationToken)
        {
            Product product = await _unitOfWork.ProductRepository.GetOneUntrackedAsync<Product>(
                filter: p => p.Id == command.Id && !p.IsDeleted,
                cancellation: cancellationToken) ?? throw new NotFoundException("Không tìm thấy sản phẩm.");

            product.IncrementViewCount();

            await _unitOfWork.ProductRepository.UpdateAsync(product);
            return true;
        }
    }
}
