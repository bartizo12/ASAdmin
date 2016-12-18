using Microsoft.Owin.Security.DataProtection;

namespace AS.Infrastructure.Web.Identity
{
    public class ASDataProtectionProvider : IDataProtectionProvider
    {
        public IDataProtector Create(params string[] purposes)
        {
            return new ASDataProtector(purposes);
        }
    }
}