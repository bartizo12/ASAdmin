using System;
using System.Collections;

namespace AS.Domain.Settings
{
    /// <summary>
    /// EventArgs to hold setting values for SettingsCacheLoaded event
    /// </summary>
    public sealed class SettingsCacheLoadedEventArgs : EventArgs
    {
        /// <summary>
        /// Dictionary of loaded settings
        /// </summary>
        private readonly IDictionary _settings;

        public IDictionary SettingValues { get { return this._settings; } }

        public SettingsCacheLoadedEventArgs(IDictionary settings)
        {
            this._settings = settings;
        }
    }
}