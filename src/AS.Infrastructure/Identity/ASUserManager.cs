using AS.Domain.Settings;
using Microsoft.AspNet.Identity;

namespace AS.Infrastructure.Identity
{
    /// <summary>
    /// Extended Microsoft.Aspnet.Identity UserManager
    /// User manager for our extended user class.
    /// </summary>
    public class ASUserManager : UserManager<ASUser, int>
    {
        public ASUserManager(ASUserStore store, ISettingManager settingManager) : base(store)
        {
            var membershipSettings = settingManager.GetContainer<MembershipSetting>();

            this.UserValidator = new UserValidator<ASUser, int>(this)
            {
                AllowOnlyAlphanumericUserNames = membershipSettings.Default.AllowOnlyAlphanumericUserNames,
                RequireUniqueEmail = membershipSettings.Default.RequireUniqueEmailForUsers
            };
            //Our password validation rules
            this.PasswordValidator = new PasswordValidator()
            {
                RequireDigit = membershipSettings.Default.RequireDigitInPassword,
                RequiredLength = membershipSettings.Default.MinimumPasswordRequiredLength,
                RequireLowercase = membershipSettings.Default.RequireLowercaseInPassword,
                RequireNonLetterOrDigit = membershipSettings.Default.RequireNonLetterOrDigitInPassword,
                RequireUppercase = membershipSettings.Default.RequireUppercaseInPassword
            };
        }

        /// <summary>
        /// Instance creater for owin IAppBuilder. Sadly , Owin config pushes us to use
        /// ServiceLocator
        /// </summary>
        /// <returns>New UserManager instance if it is registered in IOC ,othwerwise returns null</returns>
        public static ASUserManager Create()
        {
            return new ASUserManager(ServiceLocator.Current.Resolve<ASUserStore>(), ServiceLocator.Current.Resolve<ISettingManager>());
        }
    }
}