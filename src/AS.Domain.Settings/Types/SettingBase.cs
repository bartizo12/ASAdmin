namespace AS.Domain.Settings
{
    /// <summary>
    /// Base type of all setting classes
    /// </summary>
    public abstract class SettingBase
    {
        /// <summary>
        /// SettingValue Entity ID
        /// </summary>
        public int SettingValueID { get; internal set; }

        public string Comment { get; internal set; }
    }
}