using AS.Domain.Entities;
using System;
using System.ComponentModel;
using System.Globalization;

namespace AS.Domain.Settings
{
    /// <summary>
    /// Type converter for EMailSetting. Converts from SettingValue entity to EMailSetting object
    /// EMailSetting contains SMTP connection settings to be executed when an en-mail is sent.
    /// </summary>
    internal class EMailSettingTypeConverter : SettingTypeConverterBase
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            SettingValue settingValue = value as SettingValue;

            if (settingValue != null)
            {
                EMailSetting settings = new EMailSetting();
                settings.SettingValueID = settingValue.Id;
                settings.DefaultCredentials = base.GetFieldValue<bool>(settingValue, "DefaultCredentials");
                settings.EnableSsl = base.GetFieldValue<bool>(settingValue, "EnableSsl");
                settings.FromAddress = GetFieldValue<string>(settingValue, "FromAddress");
                settings.FromDisplayName = GetFieldValue<string>(settingValue, "FromDisplayName");
                settings.Host = GetFieldValue<string>(settingValue, "Host");
                settings.Name = GetFieldValue<string>(settingValue, "Name");
                settings.Password = GetFieldValue<string>(settingValue, "Password");
                settings.Port = base.GetFieldValue<int>(settingValue, "Port");
                settings.TimeOut = base.GetFieldValue<int>(settingValue, "TimeOut");
                settings.UserName = base.GetFieldValue<string>(settingValue, "UserName");
                settings.Comment = base.GetFieldValue<string>(settingValue, "Comment");

                return settings;
            }
            else
            {
                throw new InvalidCastException();
            }
        }
    }
}