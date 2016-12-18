using System.ComponentModel;

namespace AS.Domain.Settings
{
    /// <summary>
    /// AppSetting is same as appSettings in config file. Except, it is editable and cacheable
    /// Each object must have unique name.
    /// </summary>
    [TypeConverter(typeof(AppSettingTypeConverter))]
    public class AppSetting : SettingBase
    {
        public string Name { get; internal set; }
        public string Value { get; internal set; }
    }
}