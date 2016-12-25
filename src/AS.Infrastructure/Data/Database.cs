using AS.Domain.Entities;
using AS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace AS.Infrastructure.Data
{
    /// <summary>
    /// A Database adapter to execute database commands in a db-provider agnostic way
    /// </summary>
    public class Database : IDatabase
    {
        private readonly Func<DbConnectionConfiguration> _dbConnectionConfigurationFactory;

        public Database(Func<DbConnectionConfiguration> dbConnectionConfigurationFactory)
        {
            this._dbConnectionConfigurationFactory = dbConnectionConfigurationFactory;
        }
        [SuppressMessage("Microsoft.Security", "CA2100:ReviewSqlQueriesForSecurityVulnerabilities")]
        public int ExecuteNonQuery(string command, CommandType commandType, Dictionary<string, object> parameters)
        {
            var config = this._dbConnectionConfigurationFactory();

            if (config == null)
                throw new FileNotFoundException("Database connection configuration is missing");

            var factory = DbProviderFactories.GetFactory(config.DataProvider);

            using (IDbConnection connection = factory.CreateConnection())
            {
                connection.ConnectionString = config.ConnectionString;
                connection.Open();

                using (IDbCommand dbCommand = connection.CreateCommand())
                {
                    dbCommand.CommandText = command;
                    dbCommand.CommandType = commandType;

                    if (parameters != null)
                    {
                        foreach (string parameterName in parameters.Keys)
                        {
                            IDbDataParameter dbDataParameter = dbCommand.CreateParameter();
                            dbDataParameter.Value = parameters[parameterName];
                            dbDataParameter.ParameterName = parameterName;
                            dbCommand.Parameters.Add(dbDataParameter);
                        }
                    }
                    return dbCommand.ExecuteNonQuery();
                }
            }
        }
    }
}