using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nudjr_AppCore.Services.CacheServices.Implementation;
using Nudjr_AppCore.Services.CacheServices.Interfaces;
using Nudjr_AppCore.Services.CloudinaryServices.Implementation;
using Nudjr_AppCore.Services.CloudinaryServices.Interfaces;
using Nudjr_AppCore.Services.IdentityServices.Interfaces;
using Nudjr_AppCore.Services.IdentityServices.Services;
using Nudjr_AppCore.Services.Shared.Interfaces;
using Nudjr_AppCore.Services.Shared.Services;
using Nudjr_Domain.Context;
using Nudjr_Domain.Mapper;
using Nudjr_Domain.Models.IdentityModels;
using Nudjr_Persistence.UnitOfWork.Interfaces;
using Nudjr_Persistence.UnitOfWork.Services;
using System.Text.Json.Serialization;

namespace Nudjr_AppCore.Services.Extensions
{
    public static class ServiceRegistry
    {
        public static IServiceCollection ConfigureDatabaseConnection(this IServiceCollection services, IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<IdentityDatabaseContext>(options =>
               options.UseSqlServer(
                   connectionString, b => b.MigrationsAssembly("Nudjr_Api")));

            services.AddDbContext<NudjrDatabaseContext>(options =>
            {
                options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Nudjr_Api"))
                .ConfigureWarnings(c => c.Log(CoreEventId.DetachedLazyLoadingWarning, CoreEventId.LazyLoadOnDisposedContextWarning));

            });
            return services;
        }

        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services
            .AddControllers()
            .AddJsonOptions(opts =>
            {
                var enumConverter = new JsonStringEnumConverter();
                opts.JsonSerializerOptions.Converters.Add(enumConverter);
            });

            services.AddAutoMapper(opt =>
            {
                opt.AddProfile<AutoMapperProfile>();
                opt.AllowNullCollections = true;
            });

            //Identity Services
            services.TryAddScoped<UserManager<ApplicationUser>>();
            services.AddTransient<IIdentityUserService, IdentityUserService>();
            services.AddTransient<IUserRoleService, UserRoleService>();
            services.AddTransient<IUserAccountService, UserAccountService>();
            services.AddTransient<IJwtService, JwtService>();
            services.AddTransient<IRefreshTokenService, RefreshTokenService>();

            //Utility Services
            services.AddSingleton<ILoggerManager, LoggerManager>();
            services.AddTransient<IHttpClientProvider, HttpClientProvider>();
            services.AddTransient<ICacheService, CacheService>();
            services.AddTransient<IServiceFactory, ServiceFactory>();
            services.AddScoped<IUnitOfWork, UnitOfWork<NudjrDatabaseContext>>();

            services.AddTransient<INovuNotificationService, NovuNotificationService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddTransient<INudgeService, NudgeService>();
            services.AddTransient<IAlarmService, AlarmService>();

            services.AddHttpClient();


            return services;
        }
    }
}
