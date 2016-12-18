using AS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AS.Domain.Settings
{
    /// <summary>
    /// Setting manager that keeps/caches setting container in memory
    /// </summary>
    public class SettingManager : ISettingManager
    {
        private readonly Dictionary<string, dynamic> _containers;
        private readonly ISettingDataProvider _settingDataProvider;
        private readonly ITypeFinder _typeFinder;
        private readonly IStorageManager<Configuration> _configurationStorageManager;

        public SettingManager(ISettingDataProvider settingDataProvider, ITypeFinder typeFinder,
            IStorageManager<Configuration> configurationStorageManager)
        {
            this._configurationStorageManager = configurationStorageManager;
            this._settingDataProvider = settingDataProvider;
            this._typeFinder = typeFinder;
            _containers = new Dictionary<string, dynamic>();
        }

        /// <summary>
        /// Gets setting container
        /// </summary>
        /// <typeparam name="TSettingValue">Setting Type</typeparam>
        /// <returns>Setting Container</returns>
        public ISettingContainer<TSettingValue> GetContainer<TSettingValue>()
            where TSettingValue : SettingBase
        {
            string name = typeof(TSettingValue).FullName.Split('.').Last();

            if (!this._containers.Keys.Contains(name))
            {
                Type concreteType = _typeFinder.FindClassesOfType<ISettingContainer<TSettingValue>>().First();
                ISettingContainer<TSettingValue> container = Activator.CreateInstance(concreteType)
                    as ISettingContainer<TSettingValue>;

                if (!_configurationStorageManager.CheckIfExists())
                {
                    container.Load(null);
                }
                else
                {
                    container.Load(_settingDataProvider.FetchSettingValues());
                }
                this._containers.Add(name, container);
            }
            return this._containers[name] as ISettingContainer<TSettingValue>;
        }

        /// <summary>
        /// Reloads setting
        /// </summary>
        public void ReloadSettings()
        {
            if (_settingDataProvider == null)
                throw new ArgumentNullException("_settingDataProvider", "Please set _settingDataProvider.");

            var values = _settingDataProvider.FetchSettingValues();

            foreach (dynamic container in this._containers.Values)
            {
                (container as ISettingContainer).Load(values);
            }
        }
    }
}