using AS.Domain.Entities;
using System;
using System.ComponentModel;
using System.Globalization;

namespace AS.Domain.Settings
{
    /// <summary>
    /// Type converter for UrlAddresses. Converts from SettingValue entity to UrlAddress object
    /// UrlAddress is used for storing URLs. Even though these URL are mostly constant, sometimes
    /// we might want to change these URLs while application is still running.
    /// </summary>
    internal class UrlAddressTypeConverter : SettingTypeConverterBase
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            SettingValue settingValue = value as SettingValue;

            if (settingValue != null)
            {
                UrlAddress address = new UrlAddress();
                address.SettingValueID = settingValue.Id;
                address.Address = base.GetFieldValue<string>(settingValue, "Address");
                address.Name = base.GetFieldValue<string>(settingValue, "Name");
                address.Comment = base.GetFieldValue<string>(settingValue, "Comment");

                return address;
            }
            else
            {
                throw new InvalidCastException();
            }
        }
    }
}