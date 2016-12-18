using AS.Domain.Interfaces;
using AS.Domain.Settings;
using MySql.Data.Entity;
using MySql.Data.MySqlClient;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Data.Entity.SqlServerCompact;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Linq;

namespace AS.Infrastructure.Data.EF
{
    /// <summary>
    /// Custom DbConfiguration class for entity framework
    /// Note that , since the connection string is fetched from the user at the run time, this configuration does not
    /// work until connection string is provided. After connection string and provider is fetched from file , entity
    /// framework database  will be configured accordingly.
    /// For now ,it only supports  SQL Server, MySql and SQL Server Compact Edition
    /// </summary>
    public class ASDbConfiguration : DbConfiguration
    {
        public ASDbConfiguration()
        {
            IStorageManager<Configuration> connectionStringManager = ServiceLocator.Current.Resolve<IStorageManager<Configuration>>();

            if (!connectionStringManager.CheckIfExists())
                return;
            string providerName = connectionStringManager.Read().First().DataProvider;
            //If its mysql
            if (providerName == MySqlProviderInvariantName.ProviderName)
            {
                AddDependencyResolver(new MySqlDependencyResolver());
                SetProviderFactory(MySqlProviderInvariantName.ProviderName, MySqlClientFactory.Instance);
                SetProviderServices(MySqlProviderInvariantName.ProviderName, new MySqlProviderServices());
                SetDefaultConnectionFactory(new MySqlConnectionFactory());
                SetMigrationSqlGenerator(MySqlProviderInvariantName.ProviderName, () => new MySqlMigrationSqlGenerator());
#if NET_45_OR_GREATER
                SetProviderFactoryResolver(new MySqlProviderFactoryResolver());
#endif
                SetManifestTokenResolver(new MySqlManifestTokenResolver());
                SetHistoryContext(MySqlProviderInvariantName.ProviderName, (conn, schema)
                    => new ASHistoryContext(conn, schema));
            }
            else if (providerName == SqlProviderServices.ProviderInvariantName) //If Sql Server
            {
                AddDependencyResolver(SqlProviderServices.Instance);
                SetProviderFactory(SqlProviderServices.ProviderInvariantName, SqlClientFactory.Instance);
                SetProviderServices(SqlProviderServices.ProviderInvariantName, SqlProviderServices.Instance);
                SetDefaultConnectionFactory(new SqlConnectionFactory());
                SetMigrationSqlGenerator(SqlProviderServices.ProviderInvariantName, () => new SqlServerMigrationSqlGenerator());
                SetManifestTokenResolver(new DefaultManifestTokenResolver());

                SetHistoryContext(SqlProviderServices.ProviderInvariantName, (conn, schema)
                     => new System.Data.Entity.Migrations.History.HistoryContext(conn, schema));
            }
            else if (providerName == SqlCeProviderServices.ProviderInvariantName)  //If Sql Server CE
            {
                AddDependencyResolver(SqlCeProviderServices.Instance);
                SetProviderFactory(SqlCeProviderServices.ProviderInvariantName, SqlCeProviderFactory.Instance);
                SetProviderServices(SqlCeProviderServices.ProviderInvariantName, SqlCeProviderServices.Instance);
                SetDefaultConnectionFactory(new SqlCeConnectionFactory(SqlCeProviderServices.ProviderInvariantName));
                SetMigrationSqlGenerator(SqlCeProviderServices.ProviderInvariantName, () => new SqlCeMigrationSqlGenerator());
                SetManifestTokenResolver(new DefaultManifestTokenResolver());

                SetHistoryContext(SqlCeProviderServices.ProviderInvariantName, (conn, schema)
                                 => new System.Data.Entity.Migrations.History.HistoryContext(conn, schema));
            }
        }
    }
}