using AS.Domain.Settings;
using AS.Infrastructure.Data;
using AS.Infrastructure.Identity;
using AS.Infrastructure.Web.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AS.Admin
{
    public class OwinConfig
    {
        public void Configuration(IAppBuilder app)
        {
            var dbContext = DependencyResolver.Current.GetService<IDbContext>();
            var settingManager = DependencyResolver.Current.GetService<ISettingManager>();
            var membershipSettings = settingManager.GetContainer<MembershipSetting>();
            var applicationSettings = settingManager.GetContainer<AppSetting>();

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
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            if (applicationSettings.Contains("GooglePlusClientId") &&
                applicationSettings.Contains("GooglePlusClientSecret"))
            {
                app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
                {
                    ClientId = applicationSettings["GooglePlusClientId"].Value,
                    ClientSecret = applicationSettings["GooglePlusClientSecret"].Value,
                    CallbackPath = new PathString("/signin-google"),
                    Provider = new GoogleOAuth2AuthenticationProvider()
                    {
                        OnAuthenticated = context =>
                        {
                            context.Identity.AddClaim(new Claim("Google_AccessToken", context.AccessToken));

                            if (context.RefreshToken != null)
                            {
                                context.Identity.AddClaim(new Claim("GoogleRefreshToken", context.RefreshToken));
                            }
                            context.Identity.AddClaim(new Claim("GoogleUserId", context.Id));
                            context.Identity.AddClaim(new Claim("GoogleTokenIssuedAt", DateTime.Now.ToBinary().ToString()));
                            var expiresInSec = (long)(context.ExpiresIn.Value.TotalSeconds);
                            context.Identity.AddClaim(new Claim("GoogleTokenExpiresIn", expiresInSec.ToString()));

                            return Task.FromResult(0);
                        }
                    },
                    SignInAsAuthenticationType = DefaultAuthenticationTypes.ExternalCookie
                });
            }
        }
    }
}