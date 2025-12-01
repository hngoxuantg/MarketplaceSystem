using MarketplaceSystem.Application.Common.Interfaces.IDataSeedingServices;
using MarketplaceSystem.Common.Options;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Entities.Identity_Auth;
using MarketplaceSystem.Domain.Enums.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MarketplaceSystem.Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MarketplaceSystem.Infrastructure.Data.DataSeedingServices
{
    public class DataSeedingService : IDataSeedingService
    {
        private readonly MarketplaceSystemDbContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly AdminAccount _adminAccount;
        public DataSeedingService(
            IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IOptions<AdminAccount> adminAccount,
            MarketplaceSystemDbContext applicationDbContext)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _adminAccount = adminAccount.Value;
            _dbContext = applicationDbContext;
        }
        public async Task SeedDataAsync(CancellationToken cancellationToken = default)
        {
            if (!await _dbContext.Roles.AnyAsync(cancellationToken))
            {
                List<Role> roles = new List<Role>
                {
                    new Role {Name = "Admin" },
                    new Role {Name = "User" }
                };
                for (int i = 0; i < roles.Count; i++)
                    await _roleManager.CreateAsync(roles[i]);
            }
            if (!await _dbContext.Users.AnyAsync(cancellationToken))
            {
                Role? role = await _roleManager.FindByNameAsync("Admin");
                User user = new User
                {
                    UserName = _adminAccount.Account.Email,
                    Email = _adminAccount.Account.Email,
                    PhoneNumber = _adminAccount.Account.PhoneNumber,

                    LockoutEnabled = true
                };

                IdentityResult result = await _userManager.CreateAsync(user, _adminAccount.Account.Password);
                if (!result.Succeeded)
                    throw new Exception($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

                UserProfile profile = new UserProfile
                {
                    FullName = _adminAccount.Profile.FullName,
                    DateOfBirth = _adminAccount.Profile.DateOfBirth.Value,
                    Gender = (UserProfileGender)Enum.Parse(typeof(UserProfileGender), _adminAccount.Profile.Gender),
                    UserId = user.Id,
                };

                await _unitOfWork.UserProfileRepository.CreateAsync(profile);

                await _userManager.AddToRoleAsync(user, role.Name);

                string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                IdentityResult confirmResult = await _userManager.ConfirmEmailAsync(user, token);
                if (!confirmResult.Succeeded)
                    throw new Exception($"Failed to confirm admin email:" +
                        $" {string.Join(", ", confirmResult.Errors.Select(e => e.Description))}");
            }
        }
    }
}
