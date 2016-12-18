using AS.Domain.Entities;
using System;
using System.ComponentModel;
using System.Globalization;

namespace AS.Domain.Settings
{
    public class MembershipSettingsTypeConverter : SettingTypeConverterBase
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            SettingValue settingValue = value as SettingValue;

            if (settingValue != null)
            {
                MembershipSetting settings = new MembershipSetting();
                settings.SettingValueID = settingValue.Id;
                settings.AllowOnlyAlphanumericUserNames = base.GetFieldValue<bool>(settingValue, "AllowOnlyAlphanumericUserNames");
                settings.CookieValidationIntervalInMinutes = base.GetFieldValue<int>(settingValue, "CookieValidationIntervalInMinutes");
                settings.LastActivityTimeUpdateIntervalInSeconds = base.GetFieldValue<int>(settingValue, "LastActivityTimeUpdateIntervalInSeconds");
                settings.MinimumPasswordRequiredLength = base.GetFieldValue<int>(settingValue, "MinimumPasswordRequiredLength");
                settings.PasswordResetTokenExpireTimeInHours = base.GetFieldValue<int>(settingValue, "PasswordResetTokenExpireTimeInHours");
                settings.RequireDigitInPassword = base.GetFieldValue<bool>(settingValue, "RequireDigitInPassword");
                settings.RequireLowercaseInPassword = base.GetFieldValue<bool>(settingValue, "RequireLowercaseInPassword");
                settings.RequireNonLetterOrDigitInPassword = base.GetFieldValue<bool>(settingValue, "RequireNonLetterOrDigitInPassword");
                settings.RequireUniqueEmailForUsers = base.GetFieldValue<bool>(settingValue, "RequireUniqueEmailForUsers");
                settings.RequireUppercaseInPassword = base.GetFieldValue<bool>(settingValue, "RequireUppercaseInPassword");
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