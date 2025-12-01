using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Application.Common.Interfaces.IServices;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;

namespace MarketplaceSystem.Application.Features.Categories.Commands.DeleteCategory
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }
        public async Task<bool> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
        {
            return await DeleteCategoryAsync(command.Id, cancellationToken);
        }
        private async Task<bool> DeleteCategoryAsync(int id, CancellationToken cancellation = default)
        {
            Category? category = await _unitOfWork.CategoryRepository.GetByIdAsync(id, cancellation)
                ?? throw new NotFoundException("Không tìm thấy danh mục!");
            List<Category> categories = (await _unitOfWork.CategoryRepository
                .GetAllAsync<Category>(c => c.Id == id && c.ParentCategoryId == category.ParentCategoryId && c.DisplayOrder > category.DisplayOrder)).ToList();

            if (categories.Count > 0)
                foreach (var cat in categories)
                    cat.DisplayOrder -= 1;

            await _unitOfWork.CategoryRepository.UpdateRangeAsync(categories, cancellation);

            category.SetDeleted(_currentUser.UserId);
            await _unitOfWork.CategoryRepository.DeleteAsync(category, cancellation);

            return true;
        }
    }
}
