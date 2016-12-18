using AS.Domain.Entities;
using AS.Infrastructure.Data.EF;

namespace AS.Infrastructure.Mappings
{
    public class AppLogMapping : EntityTypeConfigurationBase<AppLog, long>
    {
        public AppLogMapping()
        {
            this.ToTable("AppLog");
            this.Property(l => l.Level)
                .IsRequired()
                .HasMaxLength(10);
            this.Property(l => l.Message)
                .IsRequired();
            this.Property(l => l.MachineName)
                .IsRequired()
                .HasMaxLength(255);
            this.Property(l => l.AppDomain)
                .HasMaxLength(255);
            this.Property(l => l.ClientIP)
                .HasMaxLength(20);
        }
    }
}