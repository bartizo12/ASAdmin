using AS.Domain.Entities;
using System;
using System.ComponentModel;
using System.Globalization;

namespace AS.Domain.Settings
{
    /// <summary>
    /// Type converter for EMailAddress. Converts from SettingValue entity to EMailAddress object
    /// EMailAddress is a setting that contains constant e-mail addresses to be used in the application.
    /// Such as , notifying administrator for an incident, you can keep e-mail address of the admins
    /// </summary>
    internal class EMailAddressTypeConverter : SettingTypeConverterBase
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            SettingValue settingValue = value as SettingValue;

            if (settingValue != null)
            {
                EMailAddress address = new EMailAddress();
                address.SettingValueID = settingValue.Id;
                address.Comment = base.GetFieldValue<string>(settingValue, "Comment");
                address.Name = base.GetFieldValue<string>(settingValue, "Name");
                address.Address = base.GetFieldValue<string>(settingValue, "Address");

                return address;
            }
            else
            {
                throw new InvalidCastException();
            }
        }
    }
}