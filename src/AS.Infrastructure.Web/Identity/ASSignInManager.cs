using AS.Infrastructure.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AS.Infrastructure.Web.Identity
{
    /// <summary>
    /// Custom SignInManager manages signins.
    /// The reason we implemented this here not in our core infrastructure project is that
    /// <typeparamref name="SignInManager"/> class does has dependincies on Web related references
    /// </summary>
    public class ASSignInManager : SignInManager<ASUser, int>
    {
        public ASSignInManager(ASUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ASUser user)
        {
            return user.GenerateUserIdentityAsync((ASUserManager)UserManager);
        }

        /// <summary>
        /// Instance creater for owin IAppBuilder. Sadly , Owin config pushes us to use
        /// ServiceLocator
        /// </summary>
        /// <returns>New ASSignInManager instance</returns>
        public static ASSignInManager Create()
        {
            return new ASSignInManager(ServiceLocator.Current.Resolve<ASUserManager>(),
                ServiceLocator.Current.Resolve<IAuthenticationManager>());
        }
    }
}