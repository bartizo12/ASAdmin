namespace AS.Domain.Settings
{
    public sealed class MembershipSettingContainer : SettingContainerBase<MembershipSetting>
    {
        private readonly MembershipSetting _defaultSetting;

        public MembershipSettingContainer()
        {
            _defaultSetting = new MembershipSetting()
            {
                AllowOnlyAlphanumericUserNames = true,
                CookieValidationIntervalInMinutes = 60,
                LastActivityTimeUpdateIntervalInSeconds = 60,
                MinimumPasswordRequiredLength = 4,
                Name = "MembershipSetting",
                PasswordResetTokenExpireTimeInHours = 24,
                RequireDigitInPassword = false,
                RequireLowercaseInPassword = false,
                RequireNonLetterOrDigitInPassword = false,
                RequireUniqueEmailForUsers = false,
                RequireUppercaseInPassword = false
            };
        }

        public override MembershipSetting Default
        {
            get
            {
                return base.Default ?? _defaultSetting;
            }
        }
    }
}