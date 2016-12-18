using AS.Domain.Entities;
using AS.Domain.Interfaces;
using System;
using System.Collections.Generic;

namespace AS.Services.Interfaces
{
    /// <summary>
    /// Interface for logs. Note that , this interface does not provide logging but querying/deleting logs.
    /// To log , we have ILogger interface
    /// </summary>
    public interface ILoggingService : IService
    {
        /// <summary>
        /// Filters and queries application logs
        /// </summary>
        /// <param name="logLevels">Levels to be filtered</param>
        /// <param name="from">Log record time interval from</param>
        /// <param name="to">Log record time interval to</param>
        /// <param name="pageIndex">Pagination Page Index</param>
        /// <param name="pageSize">Pagination Page Size</param>
        /// <param name="ordering">OrderBy </param>
        /// <returns>Paged list of application logs</returns>
        IPagedList<AppLog> GetLogs(List<string> logLevels, DateTime from, DateTime to, int pageIndex, int pageSize, string ordering);

        /// <summary>
        /// Gets log record by its id
        /// </summary>
        /// <param name="id">ID of the log</param>
        /// <returns>Found log record or null</returns>
        AppLog GetById(int id);

        /// <summary>
        /// Deletes all application logs
        /// </summary>
        void ClearLogs();
    }
}