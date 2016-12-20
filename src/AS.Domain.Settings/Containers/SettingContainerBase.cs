using AS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace AS.Domain.Settings
{
    /// <summary>
    /// Base setting container class . Stores settings in a dictionary which is also cached by the application
    /// by using singleton feature of the IOC container.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay("SettingName = {SettingName}")]
    public abstract class SettingContainerBase<TSetting> : IDisposable, ISettingContainer<TSetting>
        where TSetting : SettingBase, new()
    {
        #region Static

        private static readonly object SettingCacheLoadedEventKey = new object();

        #endregion Static

        #region Fields

        private readonly Dictionary<string, TSetting> settingDictionary;
        protected EventHandlerList SettingLoadedEventHanderList = new EventHandlerList();

        #endregion Fields

        #region Properties

        public string SettingName { get; private set; }

        public virtual TSetting Default
        {
            get
            {
                return this.settingDictionary.Values.FirstOrDefault();
            }
        }

        public virtual TSetting this[string key]
        {
            get
            {
                if (settingDictionary.ContainsKey(key))
                {
                    return this.settingDictionary[key];
                }
                return null;
            }
        }

        /// <summary>
        /// Event that is called when settings are loaded into the memmory cache
        /// </summary>
        public event EventHandler<SettingsCacheLoadedEventArgs> SettingCacheLoaded
        {
            add
            {
                SettingLoadedEventHanderList.AddHandler(SettingCacheLoadedEventKey, value);
            }
            remove
            {
                SettingLoadedEventHanderList.RemoveHandler(SettingCacheLoadedEventKey, value);
            }
        }

        #endregion Properties

        #region Ctor

        public SettingContainerBase()
        {
            this.SettingName = typeof(TSetting).FullName.Split('.').Last();
            settingDictionary = new Dictionary<string, TSetting>(StringComparer.InvariantCultureIgnoreCase);
        }

        #endregion Ctor

        #region Methods

        public bool Contains(string name)
        {
            return this.settingDictionary.ContainsKey(name);
        }

        public void Load(IEnumerable<SettingValue> values)
        {
            if (values == null)
                return;

            this.settingDictionary.Clear();
            IEnumerable<SettingValue> filteredValues = values.Where(sv => sv.SettingDefinition != null &&
             sv.SettingDefinition.Name.Equals(this.SettingName, StringComparison.InvariantCultureIgnoreCase));

            foreach (SettingValue value in filteredValues)
            {
                AddSettingValue(value);
            }
            OnSettingsLoaded(new SettingsCacheLoadedEventArgs(this.settingDictionary));
        }

        protected virtual void OnSettingsLoaded(SettingsCacheLoadedEventArgs e)
        {
            ((EventHandler<SettingsCacheLoadedEventArgs>)SettingLoadedEventHanderList[SettingCacheLoadedEventKey])?.Invoke(this, e);
        }

        /// <summary>
        /// This function assumes Field1 is the key field of the setting ,and adds setting with key
        /// value "Field".However,if key value is another field in the setting, better override this
        /// </summary>
        /// <param name="value"></param>
        protected virtual void AddSettingValue(SettingValue value)
        {
            if (!string.IsNullOrEmpty(value.Field1) &&
                !this.settingDictionary.ContainsKey(value.Field1))
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(TSetting));
                TSetting SettingValue = converter.ConvertFrom(value) as TSetting;
                this.settingDictionary.Add(value.Field1, SettingValue);
            }
        }

        #endregion Methods

        #region IDisposable

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (SettingLoadedEventHanderList != null)
                {
                    SettingLoadedEventHanderList.Dispose();
                    SettingLoadedEventHanderList = null;
                }
            }
            _disposed = true;
        }

        #endregion IDisposable
    }
}