using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Application.Common.Interfaces.IExternalServices.IStorageServices;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace MarketplaceSystem.Application.Features.Categories.Commands.UploadCategoryIcon
{
    public class UploadCategoryIconCommandHandler : IRequestHandler<UploadCategoryIconCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        public UploadCategoryIconCommandHandler(
            IUnitOfWork unitOfWork,
            IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public async Task<string> Handle(UploadCategoryIconCommand command, CancellationToken cancellationToken)
        {
            return await UploadCategoryIconAsync(
                command.Id,
                command.Icon,
                cancellationToken);
        }
        private async Task<string> UploadCategoryIconAsync(
            int id,
            IFormFile icon,
            CancellationToken cancellation = default)
        {
            Category? category = await _unitOfWork.CategoryRepository.GetByIdAsync(id, cancellation)
                ?? throw new NotFoundException("Danh mục không tồn tại!");

            string filePath = await _fileService.SaveImageAsync(icon, "category", cancellation);

            category.Icon = filePath;
            await _unitOfWork.CategoryRepository.UpdateAsync(category, cancellation);

            return _fileService.GetAbsoluteUrl(filePath);
        }
    }
}
