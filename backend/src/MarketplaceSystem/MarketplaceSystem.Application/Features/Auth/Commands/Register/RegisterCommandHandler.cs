using AutoMapper;
using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Entities.Identity_Auth;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace MarketplaceSystem.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        public RegisterCommandHandler(IUnitOfWork unitOfWork, UserManager<User> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }
        public async Task<Unit> Handle(RegisterCommand command, CancellationToken cancellationToken)
        {
            await RegisterAsync(command.Request, cancellationToken);
            return Unit.Value;
        }
        private async Task RegisterAsync(RegisterRequest request, CancellationToken cancellation = default)
        {
            await ValidateForRegister(request, cancellation);

            User user = _mapper.Map<User>(request);

            IdentityResult result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            UserProfile userProfile = _mapper.Map<UserProfile>(request);

            userProfile.UserId = user.Id;

            await _unitOfWork.UserProfileRepository.CreateAsync(userProfile, cancellation);

            await _userManager.AddToRoleAsync(user, "User");
        }

        private async Task ValidateForRegister(RegisterRequest registerCommand, CancellationToken cancellation = default)
        {
            if (await _unitOfWork.UserRepository.IsExistsAsync(nameof(User.Email), registerCommand.Email))
                throw new ValidatorException(nameof(RegisterRequest.Email), "Email đã tồn tại!");
        }
    }
}
