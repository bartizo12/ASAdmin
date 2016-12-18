using AS.Domain.Interfaces;
using AS.Domain.Settings;

namespace AS.Infrastructure.IO
{
    /// <summary>
    /// Stores(reads/writes) configuration
    /// </summary>
    public class ConfigurationStorageManager : StorageManagerBase<Configuration>
    {
        private const string FileName = "~/App_Data/ASConfiguration.xml";

        public ConfigurationStorageManager(IXmlSerializer xmlSerializer,
           IAppManager appManager) : base(xmlSerializer, appManager, FileName)
        {
            this.EnableCache = true;
        }
    }
}