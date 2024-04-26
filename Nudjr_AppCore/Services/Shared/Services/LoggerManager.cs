using Nudjr_AppCore.Services.Shared.Interfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_AppCore.Services.Shared.Services
{
    public class LoggerManager : ILoggerManager
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        public LoggerManager()
        {
        }

        public void LogDebug(string message)
        {
            logger.Debug(message);
        }

        public void LogError(string message, string stackTrace = "")
        {
            logger.Error(message);
        }

        public void LogError(Exception ex)
        {
            logger.Error(ex, ex.Message);
        }

        public void LogInfo(string message)
        {
            logger.Info(message);
        }

        public void LogWarn(string message)
        {
            logger.Warn(message);
        }
    }
}
