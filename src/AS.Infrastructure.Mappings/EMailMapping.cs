using AS.Domain.Entities;
using AS.Infrastructure.Data.EF;

namespace AS.Infrastructure.Mappings
{
    public class EMailMapping : EntityTypeConfigurationBase<EMail, int>
    {
        public EMailMapping()
        {
            this.ToTable("EMail");
            this.Property(e => e.EmailSettingName)
                .IsRequired()
                .HasMaxLength(300);
            this.Property(e => e.FromAddress)
                .HasMaxLength(300);
            this.Property(e => e.FromName)
                .HasMaxLength(200);
        }
    }
}