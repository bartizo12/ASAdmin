using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace AS.Infrastructure.Identity
{
    /// <summary>
    /// Custom User Claim
    /// </summary>
    [Serializable]
    public class ASUserClaim : IdentityUserClaim<int>
    {
    }
}