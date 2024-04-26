using Nudjr_Domain.Enums;
using Nudjr_Domain.Models.IdentityModels;
using Nudjr_Domain.Models.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_AppCore.Services.IdentityServices.Interfaces
{
    public interface IJwtService
    {
        Jwt GenerateJwtToken(ApplicationUser user, PersonType loggedInAs);
        Task<JwtWithRefreshToken> GenerateJWtWithRefreshTokenAsync(ApplicationUser user, PersonType loggedInAs);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
