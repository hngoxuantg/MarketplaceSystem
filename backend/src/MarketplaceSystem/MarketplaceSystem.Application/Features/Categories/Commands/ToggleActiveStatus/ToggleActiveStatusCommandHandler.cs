using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;

namespace MarketplaceSystem.Application.Features.Categories.Commands.ToggleActiveStatus
{
    public class ToggleActiveStatusCommandHandler : IRequestHandler<ToggleActiveStatusCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ToggleActiveStatusCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(ToggleActiveStatusCommand request, CancellationToken cancellationToken)
        {
            return await ToggleActiveStatusAsync(request.Id, cancellationToken);
        }

        private async Task<bool> ToggleActiveStatusAsync(int id, CancellationToken cancellation = default)
        {
            Category category = await _unitOfWork.CategoryRepository.GetOneAsync<Category>(
                filter: c => c.Id == id && !c.IsDeleted,
                cancellation: cancellation) ?? throw new NotFoundException("Danh mục không tồn tại!");

            category.ToggleActive();
            await _unitOfWork.CategoryRepository.UpdateAsync(category, cancellation);

            return true;
        }
    }
}
