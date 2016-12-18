using System.Security.Principal;

namespace AS.Domain.Interfaces
{
    /// <summary>
    /// Interface for classes that update users last activity time
    /// </summary>
    public interface ILastActivityTimeUpdater
    {
        void Update(IPrincipal principal);
    }
}