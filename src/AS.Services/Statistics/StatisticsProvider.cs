using AS.Domain.Entities;
using AS.Infrastructure.Data;
using AS.Infrastructure.Identity;
using AS.Services.Interfaces;
using System.Linq;

namespace AS.Services
{
    /// <summary>
    /// Provides statictis about the application
    /// </summary>
    public class StatisticsProvider : IStatisticsProvider
    {
        private readonly IDbContext _dbContext;

        public StatisticsProvider(IDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        /// <summary>
        /// Total user count in database
        /// </summary>
        public int TotalUserCount
        {
            get
            {
                return this._dbContext.Set<ASUser>().Count();
            }
        }

        /// <summary>
        /// Total log count in database
        /// </summary>
        public int TotalLogCount
        {
            get
            {
                return this._dbContext.Set<AppLog>().Count();
            }
        }

        /// <summary>
        /// Total e-mail count in database
        /// </summary>
        public int TotalMailCount
        {
            get
            {
                return this._dbContext.Set<EMail>().Count();
            }
        }

        /// <summary>
        /// Total async job count in database
        /// </summary>
        public int TotalJobCount
        {
            get
            {
                return this._dbContext.Set<JobDefinition>().Count();
            }
        }
    }
}