using Microsoft.AspNet.Identity;

namespace AS.Infrastructure.Identity
{
    /// <summary>
    /// Custom Role Manager
    /// </summary>
    public class ASRoleManager : RoleManager<ASRole, int>
    {
        public ASRoleManager(ASRoleStore store) : base(store)
        {
        }
    }
}