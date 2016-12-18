using AS.Domain.Interfaces;
using NLog;
using NLog.Config;
using System;

namespace AS.Infrastructure.Logging
{
    /// <summary>
    /// NLog configurator class which configures NLog according to the config file
    /// </summary>
    public static class LoggingConfigurator
    {
        /// <summary>
        /// Default config file path
        /// </summary>
        private static readonly string NLogDefaultFileName = "NLog.config";

        private static IServiceLocator _resolver;

        //Configure NLog
        public static void ConfigureNLog(IServiceLocator resolver)
        {
            _resolver = resolver;
            ConfigurationItemFactory.Default.CreateInstance = new ConfigurationItemCreator(Resolve);
            string path = AppDomain.CurrentDomain.BaseDirectory;

            if (AppDomain.CurrentDomain.BaseDirectory.EndsWith("\\"))
            {
                path += NLogDefaultFileName;
            }
            else
            {
                path += "\\" + NLogDefaultFileName;
            }
            LogManager.Configuration = new XmlLoggingConfiguration(path, false);
        }

        private static object Resolve(Type type)
        {
            if (_resolver == null)
                throw new ArgumentNullException("_resolver");

            return _resolver.Resolve(type);
        }
    }
}