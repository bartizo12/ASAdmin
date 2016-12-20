using AS.Domain.Interfaces;
using AS.Domain.Settings;
using AS.Services.Interfaces;

namespace AS.Services
{
    /// <summary>
    /// Caching service
    /// </summary>
    public class CacheService : ICacheService
    {
        private readonly ILogger _logger;
        private readonly ISettingManager _settingManager;
        private readonly IResourceManager _resourceManager;
        private readonly IConfigurationService _configurationService;

        public CacheService(ISettingManager settingManager,
            ILogger logger,
            IConfigurationService configurationService,
            IResourceManager resourceManager)
        {
            this._resourceManager = resourceManager;
            this._settingManager = settingManager;
            this._logger = logger;
            this._configurationService = configurationService;
        }

        /// <summary>
        /// Clear all cache
        /// </summary>
        public void Clear()
        {
            if (!_configurationService.CheckIfConfigExists())
                return;
            _logger.Debug("Clear all cache start");
            this._resourceManager.ClearCache();
            this._settingManager.ReloadSettings();
            _logger.Debug("Clear all cache end");
        }
    }
}