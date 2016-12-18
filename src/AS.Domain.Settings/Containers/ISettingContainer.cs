using AS.Domain.Entities;
using AS.Domain.Interfaces;
using System;
using System.Collections.Generic;

namespace AS.Domain.Settings
{
    /// <summary>
    /// Non generic settings container interface.
    /// Interface to store dynamic application setting/configuration to be cached
    /// </summary>
    public interface ISettingContainer
    {
        string SettingName { get; }

        /// <summary>
        /// Loads <paramref name="values"/> into the settings container
        /// </summary>
        /// <param name="values">Values to be loaded</param>
        void Load(IEnumerable<SettingValue> values);

        /// <summary>
        /// Checks if container contains setting value
        /// </summary>
        /// <param name="name">Name of the value to be checked</param>
        /// <returns>True if contains,false if not</returns>
        bool Contains(string name);

        /// <summary>
        /// Event that is called when settings are loaded into the memmory cache
        /// </summary>
        event EventHandler<SettingsCacheLoadedEventArgs> SettingCacheLoaded;
    }

    /// <summary>
    /// Generic setting container interface
    /// </summary>
    /// <typeparam name="TSettingValue">Setting value type</typeparam>
    public interface ISettingContainer<TSettingValue> : IHashTable<TSettingValue>,
                                              ISettingContainer where TSettingValue : SettingBase
    {
        /// <summary>
        /// Default setting value in the container
        /// </summary>
        TSettingValue Default { get; }
    }
}