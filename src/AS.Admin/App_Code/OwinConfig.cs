using AS.Domain.Settings;
using AS.Infrastructure.Data;
using AS.Infrastructure.Identity;
using AS.Infrastructure.Web.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Web.Mvc;

namespace AS.Admin
{
    public class OwinConfig
    {
        public void Configuration(IAppBuilder app)
        {
            var dbContext = DependencyResolver.Current.GetService<IDbContext>();
            var membershipSettings = DependencyResolver.Current.GetService<ISettingManager>()
                .GetContainer<MembershipSetting>();

            if (dbContext != null && dbContext.IsInitialized)
            {
                app.CreatePerOwinContext(ASUserManager.Create);
                app.CreatePerOwinContext(ASSignInManager.Create);
            }
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                CookieName = "ASAdmin",
                LogoutPath = new PathString("/Logout"),
                SlidingExpiration = true,
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Login"),
                Provider = new CookieAuthenticationProvider()
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ASUserManager, ASUser, int>
                    (
                        validateInterval: TimeSpan.FromMinutes(membershipSettings.Default.CookieValidationIntervalInMinutes),
                        regenerateIdentityCallback: (manager, user) => user.GenerateUserIdentityAsync(manager),
                        getUserIdCallback: (claim) => (int.Parse(claim.GetUserId()))
                    )
                }
            });
        }
    }
}