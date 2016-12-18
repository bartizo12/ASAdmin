using System.ComponentModel;

namespace AS.Domain.Settings
{
    /// <summary>
    /// EMailAddress is a setting definition that contains constant e-mail addresses to be used in the application.
    /// Such as , no reply e-mail address,  or  admin e-mail address ...etc
    /// </summary>
    [TypeConverter(typeof(EMailAddressTypeConverter))]
    public class EMailAddress : SettingBase
    {
        public string Name { get; internal set; }
        public string Address { get; internal set; }
    }
}