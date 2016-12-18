using AS.Domain.Entities;
using AS.Infrastructure.Data.EF;

namespace AS.Infrastructure.Mappings
{
    public class RecordAuditLogMapping : EntityTypeConfigurationBase<RecordAuditLog, long>
    {
        public RecordAuditLogMapping()
        {
            this.ToTable("RecordAuditLog");
            this.Property(l => l.Operation)
               .IsRequired()
               .HasMaxLength(20);
            this.Property(l => l.EntityName)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}