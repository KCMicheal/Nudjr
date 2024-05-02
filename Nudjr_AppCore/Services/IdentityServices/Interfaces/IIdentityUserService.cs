using Nudjr_Domain.Enums;
using Nudjr_Domain.Models.IdentityModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_AppCore.Services.IdentityServices.Interfaces
{
    public interface IIdentityUserService
    {
        Task<IdentityResult> CreateIdentityUser(ApplicationUser identityUser, PersonType personType, string pin = "default");
        Task<ApplicationUser> GetApplicationUserAsync(string identityUserId);
        Task<IdentityResult> UpdateIdentityUser(ApplicationUser identityUser);
        Task<ApplicationUser> GetApplicationUser(ClaimsPrincipal claimsPrincipal);
        Task<string> GetUserId(ClaimsPrincipal claimsPrincipal);
    }
}
