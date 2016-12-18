using AS.Infrastructure.Web.Mvc;

namespace AS.Admin.Models
{
    public class MembershipSettingModel : ASModelBase
    {
        public int SettingValueID { get; set; }
        public int PasswordResetTokenExpireTimeInHours { get; set; }
        public int LastActivityTimeUpdateIntervalInSeconds { get; set; }
        public int CookieValidationIntervalInMinutes { get; set; }
        public int MinimumPasswordRequiredLength { get; set; }
        public bool RequireUniqueEmailForUsers { get; set; }
        public bool AllowOnlyAlphanumericUserNames { get; set; }
        public bool RequireDigitInPassword { get; set; }
        public bool RequireLowercaseInPassword { get; set; }
        public bool RequireNonLetterOrDigitInPassword { get; set; }
        public bool RequireUppercaseInPassword { get; set; }
    }
}