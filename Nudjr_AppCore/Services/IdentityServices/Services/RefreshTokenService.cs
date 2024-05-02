using Nudjr_AppCore.Services.IdentityServices.Interfaces;
using Nudjr_AppCore.Services.Shared.Interfaces;
using Nudjr_Domain.Enums;
using Nudjr_Domain.Models.IdentityModels;
using Nudjr_Domain.Models.ServiceModels;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Nudjr_AppCore.Services.IdentityServices.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IServiceFactory _entityServiceFactory;
        public RefreshTokenService(IServiceFactory entityServiceFactory)
        {
            _entityServiceFactory = entityServiceFactory;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<string> GenerateAndSaveRefreshTokenAsync(string identityUserId)
        {
            string refreshToken = GenerateRefreshToken();

            IIdentityUserService identityUserService = _entityServiceFactory.GetService(typeof(IIdentityUserService)) as IIdentityUserService;
            ApplicationUser applicationUser = await identityUserService.GetApplicationUserAsync(identityUserId);
            applicationUser.RefreshToken = refreshToken;
            await identityUserService.UpdateIdentityUser(applicationUser);
            return refreshToken;
        }

        public async Task<JwtWithRefreshToken> Refresh(string token, string refreshToken)
        {

            IJwtService jwtService = _entityServiceFactory.GetService(typeof(IJwtService)) as IJwtService;
            IIdentityUserService identityUserService = _entityServiceFactory.GetService(typeof(IIdentityUserService)) as IIdentityUserService;

            ClaimsPrincipal principal = jwtService.GetPrincipalFromExpiredToken(token);
            string username = principal.Identity.Name;

            Claim loggedInAsClaim = principal.Claims.Single(x => x.Type == "LoggedInAs");

            string identityUserId = principal.Claims.Single(x => x.Type == "IdentityId").Value;
            ApplicationUser applicationUser = await identityUserService.GetApplicationUserAsync(identityUserId);

            if (applicationUser.RefreshToken != refreshToken)
                throw new SecurityTokenException("Invalid refresh token");

            Enum.TryParse<PersonType>(loggedInAsClaim.Value, out PersonType loggedInAs);
            var newJwtToken = jwtService.GenerateJwtToken(applicationUser, loggedInAs);
            var newRefreshToken = GenerateRefreshToken();
            applicationUser.RefreshToken = newRefreshToken;
            await identityUserService.UpdateIdentityUser(applicationUser);

            return new JwtWithRefreshToken
            {
                AccessToken = newJwtToken.AccessToken,
                Expires = newJwtToken.Expires,
                Issued = newJwtToken.Issued,
                RefreshToken = newRefreshToken,
            };
        }
    }
}
