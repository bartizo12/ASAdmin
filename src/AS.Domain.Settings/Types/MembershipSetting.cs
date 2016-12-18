using System.ComponentModel;

namespace AS.Domain.Settings
{
    [TypeConverter(typeof(MembershipSettingsTypeConverter))]
    public class MembershipSetting : SettingBase
    {
        public string Name { get; internal set; }
        public int PasswordResetTokenExpireTimeInHours { get; internal set; }
        public int LastActivityTimeUpdateIntervalInSeconds { get; internal set; }
        public int CookieValidationIntervalInMinutes { get; internal set; }
        public bool RequireUniqueEmailForUsers { get; internal set; }
        public bool AllowOnlyAlphanumericUserNames { get; internal set; }
        public bool RequireDigitInPassword { get; internal set; }
        public int MinimumPasswordRequiredLength { get; internal set; }
        public bool RequireLowercaseInPassword { get; internal set; }
        public bool RequireNonLetterOrDigitInPassword { get; internal set; }
        public bool RequireUppercaseInPassword { get; internal set; }
    }
}