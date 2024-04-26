using Nudjr_AppCore.Services.Shared.Interfaces;
using Microsoft.AspNetCore.Builder;
using Nudjr_Api.Infrastructure.Middlewares;

namespace Nudjr_Api.Infrastructure.StartupExtensions
{
    public static class ConfigureMiddlewares
    {

        public static WebApplication ConfigureMiddleWares(this WebApplication webApplication)
        {
            ILoggerManager? loggerManager = webApplication.Services.GetService<ILoggerManager>();

            if (loggerManager != null)
            {
                webApplication.ConfigureExceptionHandler(loggerManager);
            }
            return webApplication;
        }
    }
}
