using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace AS.Infrastructure.Identity
{
    /// <summary>
    /// Custom Role Store
    /// </summary>
    public class ASRoleStore : RoleStore<ASRole, int, ASUserRole>
    {
        public ASRoleStore(DbContext context)
            : base(context)
        {
        }
    }
}