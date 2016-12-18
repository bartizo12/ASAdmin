namespace AS.Domain.Settings
{
    /// <summary>
    /// Setting manager interface that returns settings container
    /// </summary>
    public interface ISettingManager
    {
        /// <summary>
        /// Gets setting container
        /// </summary>
        /// <typeparam name="TSettingValue">Setting Type</typeparam>
        /// <returns>Setting Container</returns>
        ISettingContainer<TSettingValue> GetContainer<TSettingValue>()
            where TSettingValue : SettingBase;

        /// <summary>
        /// Reloads setting
        /// </summary>
        void ReloadSettings();
    }
}