using AutoMapper;
using MarketplaceSystem.Application.Common.DTOs.Users;
using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Application.Common.Interfaces.IServices;
using MarketplaceSystem.Domain.Entities.Identity_Auth;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace MarketplaceSystem.Application.Features.Users.Queries.GetUserById
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(
            IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            ICurrentUserService currentUser,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _currentUser = currentUser;
            _mapper = mapper;
        }
        public async Task<UserDto> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
        {
            return await GetUserByIdAsync(query.Id, cancellationToken);
        }
        private async Task<UserDto> GetUserByIdAsync(int id, CancellationToken cancellation = default)
        {
            if (id != _currentUser.UserId)
            {
                throw new ForbiddenAccessException("Bạn không được phép truy cập vào tài nguyên này!");
            }

            User? user = await _unitOfWork.UserRepository.GetOneUntrackedAsync(
                filter: u => u.Id == id,
                selector: u => new User
                {
                    Id = u.Id,
                    Email = u.Email,
                    Profile = new Domain.Entities.Business.UserProfile
                    {
                        FullName = u.Profile != null ? u.Profile.FullName : null,
                        Avatar = u.Profile != null ? u.Profile.Avatar : null,
                    },
                },
                cancellation: cancellation);

            IList<string> roles = await _userManager.GetRolesAsync(user!);

            UserDto userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = roles.ToList();

            return userDto;
        }
    }
}
