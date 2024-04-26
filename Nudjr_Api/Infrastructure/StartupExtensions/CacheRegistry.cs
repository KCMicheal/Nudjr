using Nudjr_AppCore.Services.CacheServices.Configuration;
using StackExchange.Redis;

namespace Nudjr_Api.Infrastructure.StartupExtensions
{
    public static class CacheRegistry
    {
        private static IWebHostEnvironment environment { get; }
        public static IServiceCollection ConfigureRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            RedisConfig? redisConfig = configuration.GetSection("RedisConfig").Get<RedisConfig>();

            /*            if (environment.IsDevelopment())
                        {
                            services.AddDistributedMemoryCache();
                        }
                        else
                        {*/
            services.AddSingleton<IConnectionMultiplexer>(sp =>
             ConnectionMultiplexer.Connect(new ConfigurationOptions
             {
                 EndPoints = { $"{redisConfig.Host}:{redisConfig.Port}" },
                 AbortOnConnectFail = false,
             }));
            /*            }*/

            return services;
        }
    }
}
