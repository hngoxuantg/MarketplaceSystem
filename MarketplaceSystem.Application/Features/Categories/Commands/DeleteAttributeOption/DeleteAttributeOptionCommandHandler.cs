using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;

namespace MarketplaceSystem.Application.Features.Categories.Commands.DeleteAttributeOption
{
    public class DeleteAttributeOptionCommandHandler : IRequestHandler<DeleteAttributeOptionCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteAttributeOptionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(DeleteAttributeOptionCommand command, CancellationToken cancellationToken)
        {
            return await DeleteAttributeOptionAsync(command.CategoryId, command.AttributeId, command.OptionId, cancellationToken);
        }
        private async Task<bool> DeleteAttributeOptionAsync(int id, int attributeId, int optionId, CancellationToken cancellation = default)
        {
            await _unitOfWork.BeginTransactionAsync(cancellation);
            try
            {
                Category? category = await _unitOfWork.CategoryRepository.GetOneUntrackedAsync<Category>(
                filter: c => c.Id == id && !c.IsDeleted &&
                c.CategoryAttributes!.Any(a =>
                 a.Id == attributeId &&
                    !a.IsDeleted &&
                    a.AttributeOptions!.Any(o => o.Id == optionId && !o.IsDeleted)),
                cancellation: cancellation)
                    ?? throw new NotFoundException("Không tìm thấy danh mục!");

                AttributeOption? attributeOption = await _unitOfWork.AttributeOptionRepository.GetByIdAsync(optionId, cancellation);

                await _unitOfWork.AttributeOptionRepository.DeleteAsync(attributeOption!, cancellation);

                await _unitOfWork.CommitTransactionAsync(cancellation);

                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellation);
                throw;
            }
        }
    }
}
