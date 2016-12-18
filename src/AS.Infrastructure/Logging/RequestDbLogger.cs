using AS.Domain.Entities;
using AS.Domain.Interfaces;
using AS.Infrastructure.Data;
using System;

namespace AS.Infrastructure.Logging
{
    /// <summary>
    /// Logs client requests(HTTP requests) to a database
    /// </summary>
    public class RequestDbLogger : IRequestLogger
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly ILogger _logger;

        public RequestDbLogger(IDbContextFactory dbContextFactory, ILogger logger)
        {
            this._dbContextFactory = dbContextFactory;
            this._logger = logger;
        }

        public void Log(RequestLog log)
        {
            try
            {
                using (IDbContext dbContext = _dbContextFactory.Create())
                {
                    dbContext.Set<RequestLog>().Add(log);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}