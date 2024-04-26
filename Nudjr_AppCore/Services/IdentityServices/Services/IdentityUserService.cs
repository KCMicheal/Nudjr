using Nudjr_AppCore.Services.IdentityServices.Interfaces;
using Nudjr_Domain.Enums;
using Nudjr_Domain.Models.ExceptionModels;
using Nudjr_Domain.Models.IdentityModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Nudjr_AppCore.Services.IdentityServices.Services
{
    public class IdentityUserService : IIdentityUserService
    {
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly RoleManager<IdentityRole> RoleManager;

        public IdentityUserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        public async Task<IdentityResult> CreateIdentityUser(ApplicationUser identityUser, PersonType personType, string pin = "default")
        {
            try
            {
                IdentityResult result = await UserManager.CreateAsync(identityUser, pin);
                if (!result.Succeeded)
                {
                    string errorMessage = HandleIdentityErrors(result.Errors);
                    throw new InvalidOperationException(errorMessage);
                }

                await UserManager.AddClaimAsync(identityUser, new Claim(ClaimTypes.Email, identityUser.Email));

                await CreateApplicationRoles(RoleManager);

                await UserManager.AddToRoleAsync(identityUser, personType.ToString());
                return result;
            }
            catch (Exception ex)
            {

                throw new InvalidOperationException(ex.Message);
            }
            
        }


        private string HandleIdentityErrors(IEnumerable<IdentityError> identityErrors)
        {
            string errorMessage = "";

            foreach (var error in identityErrors)
            {
                errorMessage += $"{error.Description}. ";
            }
            return errorMessage;
        }

        public async Task<ApplicationUser> GetApplicationUserAsync(string identityUserId)
        {
            ApplicationUser applicationUser = await UserManager.FindByIdAsync(identityUserId);
            return applicationUser;
        }

        public async Task<IdentityResult> UpdateIdentityUser(ApplicationUser identityUser)
        {
            IdentityResult IdentityResult = await UserManager.UpdateAsync(identityUser);
            return IdentityResult;
        }

        public async Task<string> GetUserId(ClaimsPrincipal claimsPrincipal)
        {
            ApplicationUser user = await GetApplicationUser(claimsPrincipal);
            return user.Id;
        }

        public async Task<ApplicationUser> GetApplicationUser(ClaimsPrincipal claimsPrincipal)
        {
            string? identityUserId = UserManager.GetUserId(claimsPrincipal);
            ApplicationUser? user = await UserManager.GetUserAsync(claimsPrincipal);
            if (user == null)
                throw new NotFoundException("User Not Found");

            return user;
        }

        public async Task CreateApplicationRoles(RoleManager<IdentityRole> roleManager)
        {
            List<string> appRoles = Enum.GetNames(typeof(PersonType)).ToList();
            foreach (string role in appRoles)
            {
                bool roleExists = await roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
