using Nudjr_AppCore.Services.CacheServices.Configuration;
using Nudjr_AppCore.Services.CloudinaryServices.Configuration;
using Nudjr_Domain.Models.ConfigModels;
using Nudjr_Domain.Models.UtilityModels;

namespace Nudjr_Api.Infrastructure.StartupExtensions
{
    public static class ConfigurationRegistry
    {
        public static IServiceCollection ConfigureAppSettingsBinding(this IServiceCollection services, IConfiguration Configuration)
        {
            services.Configure<JwtConfig>(Configuration.GetSection("JwtConfig"));
            services.Configure<CommonConfig>(Configuration.GetSection("CommonConfig"));
            services.Configure<RedisConfig>(Configuration.GetSection("RedisConfig"));
            services.Configure<NovuConfig>(Configuration.GetSection("NovuConfig"));
            services.Configure<CloudinaryConfig>(Configuration.GetSection("CloudinaryConfig"));

            return services;
        }
    }
}
