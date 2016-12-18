using AS.Domain.Entities;
using System.Collections.Generic;

namespace AS.Domain.Settings
{
    /// <summary>
    /// Interface that provides settings data
    /// </summary>
    public interface ISettingDataProvider
    {
        /// <summary>
        /// Fetch Settings values from data source and returns
        /// </summary>
        /// <returns>Setting Values</returns>
        IEnumerable<SettingValue> FetchSettingValues();
    }
}