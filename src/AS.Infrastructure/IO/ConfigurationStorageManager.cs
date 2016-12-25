using AS.Domain.Entities;
using AS.Domain.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace AS.Infrastructure.IO
{
    /// <summary>
    /// Stores(reads/writes) configuration
    /// </summary>
    public class ConfigurationStorageManager : StorageManagerBase<ASConfiguration>
    {
        private const string FileName = "~/App_Data/ASConfiguration.xml";

        public ConfigurationStorageManager(IXmlSerializer xmlSerializer,
           IAppManager appManager) : base(xmlSerializer, appManager, FileName)
        {
            this.EnableCache = true;
        }
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate",Justification = "This method involves time-consuming operations", Scope = "member")]
        public Func<ASConfiguration> GetConfigurationFactory()
        {
            return () =>
            {
                if (!this.CheckIfExists())
                    return null;
                return this.Read().First();
            };
        }
    }
}