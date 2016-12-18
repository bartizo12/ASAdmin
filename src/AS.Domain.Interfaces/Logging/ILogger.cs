using System;

namespace AS.Domain.Interfaces
{
    /// <summary>
    /// Provides common interface for logging to work without depending any logging API
    /// This interface can be extended  in the future by adding some more logging functions
    /// </summary>
    public interface ILogger
    {
        void Error(Exception ex);

        void Info(string message);

        void Debug(string message);

        void Warn(string message);

        void Warn(string format, params object[] args);
    }
}