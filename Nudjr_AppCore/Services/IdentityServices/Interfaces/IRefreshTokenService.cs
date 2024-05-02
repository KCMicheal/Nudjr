using Nudjr_Domain.Models.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_AppCore.Services.IdentityServices.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<JwtWithRefreshToken> Refresh(string token, string refreshToken);
        Task<string> GenerateAndSaveRefreshTokenAsync(string identityUserId);
    }
}
