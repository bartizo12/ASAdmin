using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace AS.Infrastructure.Identity
{
    /// <summary>
    /// Custom UserRole. Since we chooses int as our Primary Key, we must create a custom
    /// UserRole class. We also extend IdentityUserRole class with additional Properties.
    /// </summary>
    public class ASUserRole : IdentityUserRole<int>
    {
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public ASUserRole()
        {
            this.CreatedOn = DateTime.UtcNow;
        }
    }
}