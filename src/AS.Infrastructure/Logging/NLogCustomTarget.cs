using AS.Domain.Entities;
using AS.Domain.Interfaces;
using AS.Infrastructure.Data;
using NLog;
using NLog.Targets;
using System;
using System.Diagnostics;
using System.IO;

namespace AS.Infrastructure.Logging
{
    /// <summary>
    /// Custom <see cref="Target"/> class for NLogger.
    /// Enriches log message with details such as ; Current Machine Name, AppDomain Name, Client IP , LoggerName ...etc
    /// and logs it to database.
    /// </summary>
    [Target("AS.Custom.NLogTarget")]
    public class NLogCustomTarget : Target
    {
        private const string FileName = "~/App_Data/Log.txt";
        private readonly IContextProvider _contextProvider;
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IAppManager _appManager;
        private readonly IXmlSerializer _xmlSerializer;

        public NLogCustomTarget(IContextProvider contextProvider,
            IDbContextFactory dbContextFactory,
            IXmlSerializer xmlSerializer,
            IAppManager appManager)
        {
            this._appManager = appManager;
            this._contextProvider = contextProvider;
            this._xmlSerializer = xmlSerializer;
            this._dbContextFactory = dbContextFactory;
        }

        protected override void Write(LogEventInfo logEvent)
        {
            AppLog log = new AppLog();
            log.AppDomain = AppDomain.CurrentDomain.FriendlyName;
            log.ClientIP = this._contextProvider.ClientIP;
            log.Level = logEvent.Level.Name;
            log.LoggerName = logEvent.LoggerName;
            log.MachineName = Environment.MachineName;
            log.Message = logEvent.FormattedMessage;

            StackTrace st = new StackTrace(true);
            StackFrame sf = this.GetRealFrame(st);

            if (sf != null)
            {
                log.Location = string.Format("{0}.{1}:{2}",
                    sf.GetMethod().ReflectedType.Name,
                    sf.GetMethod().Name,
                    sf.GetFileLineNumber());
            }
            else
                log.Location = string.Empty;

            using (IDbContext dbContext = _dbContextFactory.Create())
            {
                if (dbContext.IsInitialized)
                {
                    dbContext.Set<AppLog>().Add(log);
                    dbContext.SaveChanges();
                }
                else
                {
                    //In case , EF dbcontext is not Initialized(configured) yet , log it to file
                    log.CreatedOn = DateTime.UtcNow;
                    File.AppendAllText(_appManager.MapPhysicalFile(FileName),
                        _xmlSerializer.SerializeToXML(log));
                }
            }
        }

        private StackFrame GetRealFrame(StackTrace stackTrace)
        {
            int index;
            for (index = 0; index < stackTrace.FrameCount; index++)
            {
                StackFrame sf = stackTrace.GetFrame(index);

                if (typeof(Domain.Interfaces.ILogger).IsAssignableFrom(
                    sf.GetMethod().ReflectedType))
                {
                    break;
                }
            }

            return stackTrace.GetFrame(index + 1);
        }
    }
}