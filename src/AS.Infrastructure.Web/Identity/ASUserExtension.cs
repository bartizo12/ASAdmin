using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AS.Infrastructure.Identity
{
    public static class ASUserExtension
    {
        public static async Task<ClaimsIdentity> GenerateUserIdentityAsync(this ASUser user, ASUserManager manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(user,
                               DefaultAuthenticationTypes.ApplicationCookie);

            return userIdentity;
        }
    }
}