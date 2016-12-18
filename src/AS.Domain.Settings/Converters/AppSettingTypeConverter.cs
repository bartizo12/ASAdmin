using AS.Domain.Entities;
using System;
using System.ComponentModel;
using System.Globalization;

namespace AS.Domain.Settings
{
    /// <summary>
    /// Type converter for AppSettings. Converts from SettingValue entity to AppSetting object
    /// AppSetting has same structure as appSettings in config files. One difference in our application is that , it is editable at the runtime
    /// without causing application restart
    /// </summary>
    internal class AppSettingTypeConverter : SettingTypeConverterBase
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            SettingValue settingValue = value as SettingValue;

            if (settingValue != null)
            {
                AppSetting settings = new AppSetting();
                settings.SettingValueID = settingValue.Id;
                settings.Comment = base.GetFieldValue<string>(settingValue, "Comment");
                settings.Name = base.GetFieldValue<string>(settingValue, "Name");
                settings.Value = base.GetFieldValue<string>(settingValue, "Value");

                return settings;
            }
            else
            {
                throw new InvalidCastException();
            }
        }
    }
}