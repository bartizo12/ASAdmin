using AS.Domain.Entities;

namespace AS.Domain.Interfaces
{
    /// <summary>
    /// Logs client requests
    /// </summary>
    public interface IRequestLogger
    {
        void Log(RequestLog log);
    }
}