using AutoMapper;
using Nudjr_AppCore.Services.IdentityServices.Interfaces;
using Nudjr_AppCore.Services.Shared.Services;
using Nudjr_Domain.Enums;
using Nudjr_Domain.Models.ConfigModels;
using Nudjr_Domain.Models.IdentityModels;
using Nudjr_Domain.Models.ViewModels;
using Nudjr_Persistence.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Nudjr_AppCore.Services.IdentityServices.Services
{
    public class UserRoleService : BaseEntityService, IUserRoleService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRoleService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager) : base(unitOfWork, mapper)
        {
            _userManager = userManager;
        }

        public async Task<bool> IsUserInRole(ApplicationUser applicationUser, PersonType role)
        {
            return await _userManager.IsInRoleAsync(applicationUser, role.ToString());
        }

        public async Task<IList<ApplicationUser>> GetUsersInRole(PersonType personType)
        {
            string role = GetRole(personType);
            return await _userManager.GetUsersInRoleAsync(role);
        }

        public async Task<IList<PersonType>> GetUserRoles(ApplicationUser appUser)
        {
            IList<string> userRoles = await _userManager.GetRolesAsync(appUser);
            return GetUserPersonTypes(userRoles);
        }

        public async Task<bool> AssignUserToRole(ApplicationUser appUser, string assigneeIdentityUserId, PersonType newRole)
        {
            //Check that logged in user have sufficient privileges
            bool isUserASuperAdmin = await IsUserInRole(appUser, PersonType.SuperAdmin);
            if (!isUserASuperAdmin)
                throw new InvalidOperationException("Your Privilege Is Insufficient To Perform Action");

            ApplicationUser? assigneeUser = await _userManager.FindByIdAsync(assigneeIdentityUserId);

            //Check that User Is Not A Regular User Or Support. We do Not Want to Make Such Persons Admin
            IList<PersonType> userRoles = await GetUserRoles(assigneeUser);

            if (userRoles.Contains(PersonType.User) || userRoles.Contains(PersonType.Support))
                throw new InvalidOperationException("Operation Not Allowed. Assignee Must Not Be A Member or Support");

            if (userRoles.Contains(newRole))
                throw new InvalidOperationException("Assignee Is Already In Specified Role");

            IdentityResult result = await _userManager.AddToRoleAsync(assigneeUser, newRole.ToString());

            return result.Succeeded;
        }

        public async Task<RemoveRoleResponseModel> RemoveUserFromRole(ApplicationUser appUser, string assigneeIdentityUserId, PersonType roleToRemove)
        {
            //Check that logged in user have sufficient privileges
            bool isUserASuperAdmin = await IsUserInRole(appUser, PersonType.SuperAdmin);
            if (!isUserASuperAdmin)
                throw new InvalidOperationException("Your Privilege Is Insufficient To Perform Action");

            if (roleToRemove == PersonType.Support)
                throw new InvalidOperationException("Support Roles Cannot Be Reassigned");


            ApplicationUser? assigneeUser = await _userManager.FindByIdAsync(assigneeIdentityUserId);


            IList<PersonType> userRoles = await GetUserRoles(assigneeUser);

            //Check that User Is Not A Regular User or Support. We do Not Want to Alter their Role As they Should Play Just A Single Role
            if (userRoles.Contains(PersonType.User) || userRoles.Contains(PersonType.Support))
                throw new InvalidOperationException("Operation Not Allowed. Assignee Must Not Be A Support Or Member");

            if (!userRoles.Contains(roleToRemove))
                throw new InvalidOperationException("User Is Not In Specified Role");

            IdentityResult result = await _userManager.RemoveFromRoleAsync(assigneeUser, roleToRemove.ToString());
            PersonType personType = roleToRemove;
            if (result.Succeeded)
            {
                //Remove user role from list and return any other role the user has
                userRoles.Remove(roleToRemove);

                personType = GetUserRoleIfAny(userRoles);
            }

            return new RemoveRoleResponseModel
            {
                IsSuccess = result.Succeeded,
                RemainingRole = personType
            };
        }

        private PersonType GetUserRoleIfAny(IList<PersonType> userRoles)
        {
            PersonType personType = 0;
            if (userRoles.Contains(PersonType.User))
            {
                personType = PersonType.User;
            }
            if (userRoles.Contains(PersonType.Support))
            {
                personType = PersonType.Support;
            }
            if (userRoles.Contains(PersonType.Admin))
            {
                personType = PersonType.Admin;
            }
            if (userRoles.Contains(PersonType.SuperAdmin))
            {
                personType = PersonType.SuperAdmin;
            }

            return personType;
        }

        private string GetRole(PersonType personType)
        {
            switch (personType)
            {
                case PersonType.User:
                    return AppRoleConfig.User;
                case PersonType.Support:
                    return AppRoleConfig.Support;
                case PersonType.Admin:
                    return AppRoleConfig.Admin;
                case PersonType.SuperAdmin:
                    return AppRoleConfig.SuperAdmin;
            }
            throw new InvalidOperationException("Invalid person type");
        }

        private IList<PersonType> GetUserPersonTypes(IList<string> userRoles)
        {
            IList<PersonType> personTypes = new List<PersonType>();

            foreach (string role in userRoles)
            {
                if (role == AppRoleConfig.User)
                {
                    personTypes.Add(PersonType.User);
                }
                if (role == AppRoleConfig.Support)
                {
                    personTypes.Add(PersonType.Support);
                }
                if (role == AppRoleConfig.Admin)
                {
                    personTypes.Add(PersonType.Admin);
                }
                if (role == AppRoleConfig.SuperAdmin)
                {
                    personTypes.Add(PersonType.SuperAdmin);
                }

            }
            return personTypes;
        }
    }
}
