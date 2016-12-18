using System;
using System.Collections.Generic;

namespace AS.Domain.Entities
{
    /// <summary>
    /// User interface to build loosely coupled classes/interfaces to users
    /// </summary>
    public interface IUser : ITrackableEntity
    {
        DateTime? LastActivity { get; set; }
        DateTime? LastLogin { get; set; }
        string UserName { get; set; }
        string Email { get; set; }
        int Id { get; set; }
        List<int> RoleIds { get; }
    }
}