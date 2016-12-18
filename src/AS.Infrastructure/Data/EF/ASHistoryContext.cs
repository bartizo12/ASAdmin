using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Migrations.History;

namespace AS.Infrastructure.Data.EF
{
    /// <summary>
    /// On mysql databases , if table has compact row format , it throws
    ///  "Specified key was too long; max key length is 767 bytes" error.
    ///  To avoid this error, we need a custom HistoryContext that has key length
    ///  less than 767 bytes
    /// </summary>
    public class ASHistoryContext : HistoryContext
    {
        public ASHistoryContext(DbConnection existingConnection, string defaultSchema)
                : base(existingConnection, defaultSchema)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<HistoryRow>().Property(h => h.MigrationId).HasMaxLength(150).IsRequired();
            modelBuilder.Entity<HistoryRow>().Property(h => h.ContextKey).HasMaxLength(50).IsRequired();
        }
    }
}