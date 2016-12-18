using AS.Domain.Interfaces;
using AS.Domain.Settings;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace AS.Infrastructure.Data
{
    /// <summary>
    /// A Database adapter to execute database commands in a db-provider agnostic way
    /// </summary>
    public class Database : IDatabase
    {
        private readonly IStorageManager<Configuration> _configStorageManager;

        public Database(IStorageManager<Configuration> configStorageManager)
        {
            this._configStorageManager = configStorageManager;
        }

        [SuppressMessage("Microsoft.Security", "CA2100:ReviewSqlQueriesForSecurityVulnerabilities")]
        public int ExecuteNonQuery(string spName, Dictionary<string, object> parameters)
        {
            if (!_configStorageManager.CheckIfExists())
                throw new FileNotFoundException("config does not exist");

            Configuration config = _configStorageManager.Read().First();
            var factory = DbProviderFactories.GetFactory(config.DataProvider);

            using (IDbConnection connection = factory.CreateConnection())
            {
                connection.ConnectionString = config.ConnectionString;
                connection.Open();

                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = spName;
                    command.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        foreach (string parameterName in parameters.Keys)
                        {
                            IDbDataParameter dbDataParameter = command.CreateParameter();
                            dbDataParameter.Value = parameters[parameterName];
                            dbDataParameter.ParameterName = parameterName;
                            command.Parameters.Add(dbDataParameter);
                        }
                    }
                    return command.ExecuteNonQuery();
                }
            }
        }
    }
}