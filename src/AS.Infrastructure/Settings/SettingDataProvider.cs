using AS.Domain.Entities;
using AS.Domain.Interfaces;
using AS.Domain.Settings;
using AS.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AS.Infrastructure
{
    /// <summary>
    /// Setting data provider class that fetches data from database via EF interface and returns
    /// </summary>
    public class SettingDataProvider : ISettingDataProvider
    {
        private readonly IDbContextFactory _dbContextFactory;

        public SettingDataProvider(IDbContextFactory dbContextFactory)
        {
            this._dbContextFactory = dbContextFactory;
        }

        /// <summary>
        /// Fetch Settings values from data source and returns
        /// </summary>
        /// <returns>Setting Values</returns>
        public IEnumerable<SettingValue> FetchSettingValues()
        {
            try
            {
                using (IDbContext dbContext = this._dbContextFactory.Create())
                {
                    return dbContext.Set<SettingValue>().Include("SettingDefinition").ToList();
                }
            }
            catch
            {
                return null;
            }
        }
    }
}