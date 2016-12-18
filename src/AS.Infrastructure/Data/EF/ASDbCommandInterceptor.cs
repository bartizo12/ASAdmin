using AS.Domain.Interfaces;
using AS.Domain.Settings;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Diagnostics;

namespace AS.Infrastructure.Data.EF
{
    /// <summary>
    /// DbCommand Interceptor to log db commands that are executed by EF.
    /// Logging can be enabled/disabled by DbQueryLogEnable setting that is defined in AppSettings
    /// Logs will be inserted into DbCommand table
    /// </summary>
    public class ASDbCommandInterceptor : IDbCommandInterceptor
    {
        private readonly IDatabase _database;
        private readonly ISettingManager _settingManager;
        private readonly IContextProvider _contextProvider;

        public ASDbCommandInterceptor(IDatabase database,
            IContextProvider contextProvider,
            ISettingManager settingManager)
        {
            this._database = database;
            this._contextProvider = contextProvider;
            this._settingManager = settingManager;
        }

        public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
        }

        public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
        }

        public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            this.Executed<DbDataReader>(command, interceptionContext);
        }

        public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            this.Executing<DbDataReader>(interceptionContext);
        }

        public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            this.Executed<object>(command, interceptionContext);
        }

        public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            this.Executing<object>(interceptionContext);
        }

        private void Executing<T>(DbCommandInterceptionContext<T> interceptionContext)
        {
            if (!interceptionContext.IsAsync
                && _settingManager.GetContainer<AppSetting>().Contains("DbQueryLogEnable")
                && bool.Parse(_settingManager.GetContainer<AppSetting>()["DbQueryLogEnable"].Value))
            {
                Stopwatch timer = new Stopwatch();
                interceptionContext.UserState = timer;
                timer.Start();
            }
        }

        private void Executed<T>(DbCommand command, DbCommandInterceptionContext<T> interceptionContext)
        {
            if (!interceptionContext.IsAsync
                && _settingManager.GetContainer<AppSetting>().Contains("DbQueryLogEnable")
                 && bool.Parse(_settingManager.GetContainer<AppSetting>()["DbQueryLogEnable"].Value))
            {
                Stopwatch timer = (Stopwatch)interceptionContext.UserState;
                timer.Stop();
                string error = string.Empty;

                if (interceptionContext.Exception != null)
                    error = interceptionContext.Exception.ToString();

                if (!string.IsNullOrEmpty(error) && !command.CommandText.StartsWith("INSERT"))
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>();
                    parameters.Add("@command", command.CommandText);
                    parameters.Add("@duration", timer.ElapsedMilliseconds);
                    parameters.Add("@error", error);
                    parameters.Add("@createdOn", DateTime.UtcNow);

                    if (_contextProvider != null)
                        parameters.Add("@createdBy", _contextProvider.UserName);
                    else
                        parameters.Add("@createdBy", string.Empty);

                    this._database.ExecuteNonQuery("DbCommand_INS", parameters);
                }
            }
        }
    }
}