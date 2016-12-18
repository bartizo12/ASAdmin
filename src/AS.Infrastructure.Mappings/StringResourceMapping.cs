using AS.Domain.Entities;
using AS.Infrastructure.Data.EF;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace AS.Infrastructure.Mappings
{
    public class StringResourceMapping : EntityTypeConfigurationBase<StringResource, int>
    {
        public StringResourceMapping()
        {
            this.ToTable("StringResource");
            this.Property(l => l.CultureCode)
                .IsRequired()
                .HasMaxLength(10)
                .HasUniqueIndexAnnotation("UQ_CultureCode_Name_PerApp", 0);

            this.Property(l => l.Name)
                .IsRequired()
                .HasMaxLength(200)
                .HasUniqueIndexAnnotation("UQ_CultureCode_Name_PerApp", 1);

            this.Property(l => l.Value)
                .IsRequired();
        }
    }
}