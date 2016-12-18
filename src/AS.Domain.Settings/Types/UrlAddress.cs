using System.ComponentModel;

namespace AS.Domain.Settings
{
    /// UrlAddress is used for storing URLs. Even though these URL are mostly constant, sometimes
    /// we might want to change these URLs while application is still running.
    [TypeConverter(typeof(UrlAddressTypeConverter))]
    public class UrlAddress : SettingBase
    {
        /// <summary>
        /// Name of the URL.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// URL
        /// </summary>
        public string Address { get; internal set; }
    }
}