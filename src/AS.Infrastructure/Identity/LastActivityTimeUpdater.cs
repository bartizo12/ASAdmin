using AS.Domain.Interfaces;
using AS.Domain.Settings;
using AS.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Principal;

namespace AS.Infrastructure.Identity
{
    /// <summary>
    /// Updates last activity time of the online users.
    /// Caches times in <see cref="MemoryCache"/> and updates users
    /// every 60 seconds(can be changed via membership setttings) to avoid high database traffic.
    /// </summary>
    public class LastActivityTimeUpdater : ILastActivityTimeUpdater
    {
        private static DateTime lastLogTime = DateTime.UtcNow;

        private const string CacheKeyName = "LastActivity";
        private readonly IDbContext _dbContext;
        private readonly ILogger _logger;
        private readonly ISettingManager _settingManager;

        public LastActivityTimeUpdater(IDbContext dbContext, ILogger logger, ISettingManager settingManager)
        {
            this._logger = logger;
            this._dbContext = dbContext;
            this._settingManager = settingManager;
        }

        public void Update(IPrincipal principal)
        {
            try
            {
                Dictionary<string, DateTime> lastActivities = (Dictionary<string, DateTime>)MemoryCache.Default.Get(CacheKeyName);

                if (principal.Identity.IsAuthenticated)
                {
                    string user = principal.Identity.Name;

                    if (lastActivities == null)
                        lastActivities = new Dictionary<string, DateTime>();

                    if (!lastActivities.ContainsKey(user))
                        lastActivities.Add(user, DateTime.UtcNow);
                    else
                        lastActivities[user] = DateTime.UtcNow;

                    if (MemoryCache.Default.Contains(CacheKeyName))
                    {
                        MemoryCache.Default[CacheKeyName] = lastActivities;
                    }
                    else
                    {
                        MemoryCache.Default.Add(CacheKeyName, lastActivities, DateTimeOffset.MaxValue);
                    }
                }
                if (lastActivities != null && DateTime.UtcNow.Subtract(lastLogTime).TotalSeconds > _settingManager.GetContainer<MembershipSetting>().Default.LastActivityTimeUpdateIntervalInSeconds)
                {
                    if (_dbContext.IsInitialized)
                    {
                        foreach (string userName in lastActivities.Keys)
                        {
                            ASUser user = _dbContext.Set<ASUser>().SingleOrDefault(u => u.UserName == userName);

                            if (user != null && lastActivities.ContainsKey(userName))
                            {
                                user.LastActivity = lastActivities[userName];
                                _dbContext.Entry(user).State = EntityState.Modified;
                            }
                        }
                        _dbContext.SaveChanges();
                    }
                    lastActivities.Clear();
                    lastLogTime = DateTime.UtcNow;
                }
            }
            catch (Exception ex)
            {
                this._logger.Error(ex);
            }
        }
    }
}