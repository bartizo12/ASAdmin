using AS.Domain.Settings;
using NLog;
using System;

namespace AS.Infrastructure.Logging
{
    /// <summary>
    /// Logger class that wraps NLog.Logger.
    /// This class has implemented to make our application dependent on <see cref="ILogger"/> interface rather than
    /// NLog.Logger itself.In other words, it makes NLog.Logger injectable by IOC.
    /// </summary>
    public class NLogger : BaseLogger, Domain.Interfaces.ILogger
    {
        private readonly Logger logger;

        public NLogger(ISettingManager settingManager)
            : base(settingManager)
        {
            this.logger = LogManager.GetCurrentClassLogger();
        }
        public void Error(Exception ex)
        {
            this.logger.Error(ex);
        }
        public void Info(string message)
        {
            if (MinimumLogLevel <= Domain.Interfaces.LogLevel.Info)
            {
                this.logger.Info(message);
            }
        }
        public void Debug(string message)
        {
            if (MinimumLogLevel <= Domain.Interfaces.LogLevel.Debug)
            {
                this.logger.Debug(message);
            }
        }
        public void Warn(string message)
        {
            if (MinimumLogLevel <= Domain.Interfaces.LogLevel.Warn)
            {
                this.logger.Warn(message);
            }
        }
        public void Warn(string format, params object[] args)
        {
            if (MinimumLogLevel <= Domain.Interfaces.LogLevel.Warn)
            {
                this.logger.Warn(format, args);
            }
        }
    }
}