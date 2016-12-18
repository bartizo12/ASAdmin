using AS.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AS.Infrastructure.Data.EF
{
    /// <summary>
    /// Base EntityFramework mapping configuration class
    /// </summary>
    /// <typeparam name="TEntity">Generic entity type</typeparam>
    /// <typeparam name="TID">Generic ID type</typeparam>
    public abstract class EntityTypeConfigurationBase<TEntity, TID> : EntityTypeConfiguration<TEntity>
        where TEntity : EntityBase<TID> where TID : struct
    {
        protected EntityTypeConfigurationBase()
        {
            this.HasKey(l => l.Id);
            this.Property(l => l.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(l => l.CreatedBy)
                .HasMaxLength(255);
        }
    }
}