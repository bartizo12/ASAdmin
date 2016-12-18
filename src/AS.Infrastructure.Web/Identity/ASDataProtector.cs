using Microsoft.Owin.Security.DataProtection;
using System.Web.Security;

namespace AS.Infrastructure.Web.Identity
{
    public class ASDataProtector : IDataProtector
    {
        private readonly string[] _purposes;

        public ASDataProtector(string[] purposes)
        {
            _purposes = purposes;
        }

        public byte[] Protect(byte[] userData)
        {
            return MachineKey.Protect(userData, _purposes);
        }

        public byte[] Unprotect(byte[] protectedData)
        {
            return MachineKey.Unprotect(protectedData, _purposes);
        }
    }
}