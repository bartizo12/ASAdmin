using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace AS.Infrastructure.Identity
{
    /// <summary>
    /// Custom UserStore
    /// </summary>
    public class ASUserStore : UserStore<ASUser, ASRole, int, ASUserLogin, ASUserRole, ASUserClaim>
    {
        public ASUserStore(DbContext context)
            : base(context)
        {
        }
    }
}