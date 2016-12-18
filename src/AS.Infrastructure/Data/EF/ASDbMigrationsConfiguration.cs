using System.Data.Entity.Migrations;

namespace AS.Infrastructure.Data.EF
{
    /// <summary>
    /// Custom DbMigration configuration
    /// </summary>
    internal class ASDbMigrationsConfiguration : DbMigrationsConfiguration<ASDbContext>
    {
        public ASDbMigrationsConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = false;
            ContextKey = "AS";
        }
    }
}