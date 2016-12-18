using AS.Domain.Entities;
using AS.Domain.Interfaces;
using AS.Infrastructure.Collections;
using AS.Infrastructure.Data;
using AS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace AS.Services
{
    /// <summary>
    /// Deals with log records (Select/Delete).To do logging , we have ILogger interface
    /// </summary>
    public class LoggingService : ILoggingService
    {
        private readonly IDbContext _dbContext;

        public LoggingService(IDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

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
        public IPagedList<AppLog> GetLogs(List<string> logLevels, DateTime from, DateTime to, int pageIndex, int pageSize, string ordering)
        {
            var query = this._dbContext.Set<AppLog>().AsNoTracking() as IQueryable<AppLog>;
            query = query.Where(log => log.CreatedOn >= from && log.CreatedOn <= to);
            query = query.Where(log => logLevels.Contains(log.Level));

            if (!string.IsNullOrEmpty(ordering))
            {
                query = query.OrderBy(ordering);
            }

            return new PagedList<AppLog>(query, pageIndex, pageSize);
        }

        /// <summary>
        /// Deletes all application logs
        /// </summary>
        public void ClearLogs()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets log record by its id
        /// </summary>
        /// <param name="id">ID of the log</param>
        /// <returns>Found log record or null</returns>
        public AppLog GetById(int id)
        {
            return _dbContext.Set<AppLog>().Single(appLog => appLog.Id == id);
        }
    }
}