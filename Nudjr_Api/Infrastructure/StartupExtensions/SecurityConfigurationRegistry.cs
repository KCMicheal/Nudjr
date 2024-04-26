using Nudjr_Domain.Context;
using Nudjr_Domain.Models.ConfigModels;
using Nudjr_Domain.Models.IdentityModels;
using Nudjr_Domain.Models.UtilityModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Nudjr_Api.Infrastructure.StartupExtensions
{
    public static class SecurityConfigurationRegistry
    {
        public static IServiceCollection ConfigureIdentityProvider(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDefaultIdentity<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<IdentityDatabaseContext>()
                .AddDefaultTokenProviders();

            CommonConfig? lockoutConfig = configuration.GetSection("CommonConfig").Get<CommonConfig>();

            services.Configure<IdentityOptions>(config =>
            {
                // User defined password policy settings.  
                config.SignIn.RequireConfirmedEmail = true;

                // Password settings
                config.Password.RequireDigit = false;
                config.Password.RequiredLength = 6;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
                config.Password.RequireLowercase = false;
                config.Password.RequiredUniqueChars = 1;

                // Lockout settings
                config.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(lockoutConfig.FailedLoginLockoutDurationInMinutes);
                config.Lockout.MaxFailedAccessAttempts = lockoutConfig.MaxFailedLoginAttemptCount;
                config.Lockout.AllowedForNewUsers = true;
                // User settings
                config.User.RequireUniqueEmail = true;

            });


            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsLoggedInAsMember", policy => policy.RequireClaim("LoggedInAs", AppRoleConfig.User));
                options.AddPolicy("IsLoggedInAsSupport", policy => policy.RequireClaim("LoggedInAs", AppRoleConfig.Support));
                options.AddPolicy("IsLoggedInAsAdmin", policy => policy.RequireClaim("LoggedInAs", AppRoleConfig.Admin));
                options.AddPolicy("IsLoggedInAsSuperAdmin", policy => policy.RequireClaim("LoggedInAs", AppRoleConfig.SuperAdmin));

            });

            return services;
        }

        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration Configuration)
        {
            JwtConfig jwtConfig = Configuration.GetSection("JwtConfig").Get<JwtConfig>();

            var key = Encoding.ASCII.GetBytes(jwtConfig.JwtKey);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true
                };
            });

            return services;
        }
    }
}
