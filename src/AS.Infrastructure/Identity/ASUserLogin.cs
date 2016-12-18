using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace AS.Infrastructure.Identity
{
    [Serializable]
    public class ASUserLogin : IdentityUserLogin<int>
    {
    }
}