using AS.Domain.Interfaces;
using AS.Domain.Settings;
using MySql.Data.MySqlClient;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;

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
        #region Constants

        private const string SqlServerScriptPath = "~/SQLScripts/SQLServer.sql";
        private const string MySqlScriptPath = "~/SQLScripts/MySql.sql";
        private const string SqlServerEOC = "GO";
        private const string MySqlEOC = ";";

        #endregion Constants

        private readonly IStorageManager<Configuration> _configurationStorageManager;
        private readonly ASDbMigrationsConfiguration _config;
        private readonly IAppManager _appManager;

        public ASDatabaseInitializer(IStorageManager<Configuration> configurationStorageManager,
            IAppManager appManager)
        {
            this._appManager = appManager;
            this._configurationStorageManager = configurationStorageManager;
            this._config = new ASDbMigrationsConfiguration();
        }

        public void InitializeDatabase(TDbContext context)
        {
            if (!_configurationStorageManager.CheckIfExists())
                return;

            var config = this._configurationStorageManager.Read().First();
            _config.TargetDatabase = new DbConnectionInfo(config.ConnectionString, config.DataProvider);

            var migrator = new DbMigrator(_config);
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