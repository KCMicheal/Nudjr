using Nudjr_Domain.Entities;
using Nudjr_Domain.Enums;
using Nudjr_Domain.Models.IdentityModels;
using Nudjr_Domain.Models.ServiceModels;
using Nudjr_Domain.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_AppCore.Services.IdentityServices.Interfaces
{
    public interface IUserAccountService
    {
        Task<Guid> GetUserId(ClaimsPrincipal claimsPrincipal);
        Task<USER> GetUser(ClaimsPrincipal claimsPrincipal);
        Task<USER> GetUserByEmail(string emailAddress);
        Task<bool> AssignUserToRole(ApplicationUser appUser, Guid assigneeUserId, PersonType newRole);
        Task<USER> CreateUserAccount(UserSignUpDto model, string userName, string pin);
        Task<USER> CreateAdminAccount(UserSignUpDto model);
        Task<Jwt> GenerateLoginTokenAfterAccountCreation(USER user);
        Task<JwtWithRefreshToken> UserLogin(string userName, string password, PersonType personType);
        Task<Jwt> AdminLogin(string userName, string password);
        Task<JwtWithRefreshToken> RefreshToken(string token, string refreshToken);
    }
}
