using AutoMapper;
using MarketplaceSystem.Application.Common.DTOs.Categories;
using MarketplaceSystem.Application.Common.Interfaces.IExternalServices.IStorageServices;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;

namespace MarketplaceSystem.Application.Features.Categories.Queries.GetHomeCategories
{
    public class GetHomeCategoriesQueryHandler : IRequestHandler<GetHomeCategoriesQuery, RootCategoryCardDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        public GetHomeCategoriesQueryHandler(IUnitOfWork unitOfWork, IFileService fileService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _mapper = mapper;
        }
        public async Task<RootCategoryCardDto> Handle(GetHomeCategoriesQuery query, CancellationToken cancellationToken)
        {
            return await GetHomeCategoriesAsync(cancellationToken);
        }
        private async Task<RootCategoryCardDto> GetHomeCategoriesAsync(CancellationToken cancellation = default)
        {
            (IEnumerable<Category> categories, int totalCount) = await _unitOfWork.CategoryRepository.GetPagedAsync(
                filter: c => c.ParentCategoryId == null && c.IsActive && !c.IsDeleted,
                orderBy: q => q.OrderBy(q => q.DisplayOrder),
                pageNumber: 1,
                pageSize: 15,
                cancellationToken: cancellation
                );


            List<RootCategoryItemListDto> categoryItemListDto = _mapper.Map<IEnumerable<RootCategoryItemListDto>>(categories).ToList();

            foreach (var categoryItem in categoryItemListDto)
            {
                if (!string.IsNullOrEmpty(categoryItem.Icon))
                {
                    categoryItem.Icon = _fileService.GetAbsoluteUrl(categoryItem.Icon);
                }
            }

            RootCategoryCardDto result = new RootCategoryCardDto
            {
                TotalCount = totalCount,
                Items = categoryItemListDto
            };

            return result;
        }
    }
}
