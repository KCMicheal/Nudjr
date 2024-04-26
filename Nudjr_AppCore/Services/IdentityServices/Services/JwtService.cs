using Nudjr_AppCore.Services.IdentityServices.Interfaces;
using Nudjr_AppCore.Services.Shared.Interfaces;
using Nudjr_AppCore.Services.Shared.Services;
using Nudjr_Domain.Enums;
using Nudjr_Domain.Models.IdentityModels;
using Nudjr_Domain.Models.ServiceModels;
using Nudjr_Domain.Models.UtilityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Nudjr_Domain.Models.ConfigModels;

namespace Nudjr_AppCore.Services.IdentityServices.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtConfig _jwtConfig;
        private readonly IServiceFactory ServiceFactory;
        UserManager<ApplicationUser> UserManager { get; set; }
        RoleManager<IdentityRole> RoleManager { get; set; }
        public JwtService(IServiceFactory serviceFactory, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, IOptionsSnapshot<JwtConfig> jwtConfig)
        {
            ServiceFactory = serviceFactory;
            UserManager = userManager;
            RoleManager = roleManager;
            _jwtConfig = jwtConfig.Value;
        }

        public Jwt GenerateJwtToken(ApplicationUser user, PersonType personType)
        {
            string tokenIdentity = Guid.NewGuid().ToString();
            var currentDateTime = DateTime.Now;
            var claims = GetValidClaims(user, currentDateTime, personType.ToString()).GetAwaiter().GetResult();
            claims.Add(new Claim("TokenIdentity", tokenIdentity));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.JwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = currentDateTime.AddMinutes(Convert.ToInt64(_jwtConfig.JwtExpireInMinutes));

            JwtSecurityToken token = new JwtSecurityToken(
                _jwtConfig.JwtIssuer,
                _jwtConfig.JwtAudience,
                claims,
                expires: expires,
                signingCredentials: creds

            );

            string encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);

            return new Jwt
            {
                AccessToken = encodedJwt,
                Issued = currentDateTime,
                Expires = expires,
                TokenIdentity = tokenIdentity,

            };
        }

        public async Task<JwtWithRefreshToken> GenerateJWtWithRefreshTokenAsync(ApplicationUser user, PersonType loggedInAs)
        {
            IRefreshTokenService refreshTokenService = ServiceFactory.GetService(typeof(IRefreshTokenService)) as IRefreshTokenService;


            var jwtToken = GenerateJwtToken(user, loggedInAs);
            var refreshToken = await refreshTokenService.GenerateAndSaveRefreshTokenAsync(user.Id);
            return new JwtWithRefreshToken
            {
                AccessToken = jwtToken.AccessToken,
                Expires = jwtToken.Expires,
                Issued = jwtToken.Issued,
                RefreshToken = refreshToken,
            };
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.JwtKey));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        private async Task<List<Claim>> GetValidClaims(ApplicationUser user, DateTime dateIssued, string loggedInAs)
        {
            IdentityOptions _options = new IdentityOptions();
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(dateIssued).ToString(), ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.NameIdentifier, user.UserName),
                new Claim("IdentityId", user.Id.ToString()),
                new Claim("LoggedInAs", loggedInAs)
            };

            var userClaims = await UserManager.GetClaimsAsync(user);

            claims.AddRange(userClaims);


            var userRoles = await UserManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await RoleManager.FindByNameAsync(userRole);
                if (role != null)
                {
                    var roleClaims = await RoleManager.GetClaimsAsync(role);
                    foreach (Claim roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }
            return claims;
        }

        private object ToUnixEpochDate(DateTime dateIssued)
        {
            var timestamp = (dateIssued - new DateTime(1970, 01, 01)).TotalMilliseconds;
            return timestamp;
        }
    }
}
