using AS.Domain.Entities;
using AS.Domain.Interfaces;
using MySql.Data.MySqlClient;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;

namespace AS.Infrastructure.Data.EF
{
    /// <summary>
    /// Custom DatabaseInitializer.
    /// Initializes database and migrates to the latest version.
    /// During initalization For SqlServer/SqlServerCE executes "~/SQLScripts/SQLServer.sql" and
    /// for MySql executes  "~/SQLScripts/MySql.sql"
    /// These script files can be modified, if needed
    /// </summary>
    /// <typeparam name="TDbContext">Entity Framework DbContext</typeparam>
    public class ASDatabaseInitializer<TDbContext> : IDatabaseInitializer<TDbContext>
        where TDbContext : DbContext
    {
        private const string SqlServerScriptPath = "~/SQLScripts/SQLServer.sql";
        private const string MySqlScriptPath = "~/SQLScripts/MySql.sql";
        private const string SqlServerEOC = "GO";
        private const string MySqlEOC = ";";

        private readonly Func<DbConnectionConfiguration> _dbConnectionConfigurationFactory;
        private readonly ASDbMigrationsConfiguration _migrationConfig;
        private readonly IAppManager _appManager;

        public ASDatabaseInitializer(Func<DbConnectionConfiguration> dbConnectionConfigurationFactory,
            IAppManager appManager)
        {
            this._appManager = appManager;
            this._dbConnectionConfigurationFactory = dbConnectionConfigurationFactory;
            this._migrationConfig = new ASDbMigrationsConfiguration();
        }

        public void InitializeDatabase(TDbContext context)
        {
            var config = _dbConnectionConfigurationFactory();
            if (config == null)
                return;

            _migrationConfig.TargetDatabase = new DbConnectionInfo(config.ConnectionString, config.DataProvider);

            var migrator = new DbMigrator(_migrationConfig);
            migrator.Update();
            string scriptPath, eoc;

            if (context.Database.Connection.GetType() == typeof(MySqlConnection))
            {
                scriptPath = _appManager.MapPhysicalFile(MySqlScriptPath);
                eoc = MySqlEOC;
            }
            else
            {
                scriptPath = _appManager.MapPhysicalFile(SqlServerScriptPath);
                eoc = SqlServerEOC;
            }

            SqlCommandParser parser = new SqlCommandParser(scriptPath, eoc);

            foreach (string command in parser.ParseFromFile(false))
            {
                context.Database.ExecuteSqlCommand(command);
            }
        }
    }
}