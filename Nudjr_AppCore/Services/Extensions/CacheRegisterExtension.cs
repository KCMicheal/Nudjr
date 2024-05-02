using Nudjr_AppCore.Services.CacheServices.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_AppCore.Services.Extensions
{
    public static class CacheRegisterExtension
    {
        public static void AddRedisCache(this IServiceCollection services, RedisConfig redisConfig)
        {
            ConfigurationOptions configurationOptions = new ConfigurationOptions();
            configurationOptions.SslProtocols = SslProtocols.Tls12;
            configurationOptions.SyncTimeout = 30000;
            configurationOptions.Ssl = true;
            configurationOptions.Password = redisConfig.Password;
            configurationOptions.AbortOnConnectFail = false;
            configurationOptions.EndPoints.Add(redisConfig.Host, redisConfig.Port);

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configurationOptions.ToString();
                options.InstanceName = redisConfig.InstanceId;
            });

            services.AddSingleton<IConnectionMultiplexer>((x) =>
            {
                var connectionMultiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions
                {
                    Password = configurationOptions.Password,
                    EndPoints = { configurationOptions.EndPoints[0] },
                    AbortOnConnectFail = false,
                    AllowAdmin = false,
                    ClientName = redisConfig.InstanceId
                });
                return connectionMultiplexer;
            });
        }
    }

}
