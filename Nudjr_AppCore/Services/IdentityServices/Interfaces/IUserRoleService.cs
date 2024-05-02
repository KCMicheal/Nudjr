using Nudjr_Domain.Enums;
using Nudjr_Domain.Models.IdentityModels;
using Nudjr_Domain.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_AppCore.Services.IdentityServices.Interfaces
{
    public interface IUserRoleService
    {
        Task<bool> IsUserInRole(ApplicationUser applicationUser, PersonType role);
        Task<IList<ApplicationUser>> GetUsersInRole(PersonType personType);
        Task<IList<PersonType>> GetUserRoles(ApplicationUser appUser);
        Task<bool> AssignUserToRole(ApplicationUser appUser, string assigneeIdentityUserId, PersonType newRole);
        Task<RemoveRoleResponseModel> RemoveUserFromRole(ApplicationUser appUser, string assigneeIdentityUserId, PersonType roleToRemove);
    }
}
