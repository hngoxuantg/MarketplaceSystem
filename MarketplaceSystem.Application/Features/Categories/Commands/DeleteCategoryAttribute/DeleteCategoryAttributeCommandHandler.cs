using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Application.Common.Interfaces.IServices;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;

namespace MarketplaceSystem.Application.Features.Categories.Commands.DeleteCategoryAttribute
{
    public class DeleteCategoryAttributeCommandHandler : IRequestHandler<DeleteCategoryAttributeCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        public DeleteCategoryAttributeCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }
        public async Task<bool> Handle(DeleteCategoryAttributeCommand command, CancellationToken cancellationToken)
        {
            return await DeleteCategoryAttributeAsync(command.CategoryId, command.AttributeId, cancellationToken);
        }
        private async Task<bool> DeleteCategoryAttributeAsync(int id, int attributeId, CancellationToken cancellation = default)
        {
            await _unitOfWork.BeginTransactionAsync(cancellation);

            try
            {
                Category? category = await _unitOfWork.CategoryRepository.GetOneUntrackedAsync<Category>(
                filter: c => (c.Id == id && !c.IsDeleted) && (c.CategoryAttributes!.Any(a => a.Id == attributeId && !a.IsDeleted)), cancellation: cancellation)
                    ?? throw new NotFoundException("Không tìm thấy danh mục!");

                CategoryAttribute? categoryAttribute = await _unitOfWork.CategoryAttributeRepository.GetByIdAsync(attributeId, cancellation);

                List<CategoryAttribute> categoryAttributes = (await _unitOfWork.CategoryAttributeRepository
                    .GetAllAsync<CategoryAttribute>(a => a.CategoryId == id && a.Id != attributeId && a.DisplayOrder > categoryAttribute.DisplayOrder)).ToList();

                if (categoryAttributes.Count > 0)
                    foreach (var attr in categoryAttributes)
                        attr.DisplayOrder -= 1;

                categoryAttribute.SetDeleted(_currentUser.UserId);
                _unitOfWork.CategoryAttributeRepository.UpdateRangeEntity(categoryAttributes);

                await _unitOfWork.SaveChangesAsync(cancellation);

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
