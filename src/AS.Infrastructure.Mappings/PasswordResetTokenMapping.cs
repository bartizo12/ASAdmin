using AS.Domain.Entities;
using AS.Infrastructure.Data.EF;

namespace AS.Infrastructure.Mappings
{
    public class PasswordResetTokenMapping : EntityTypeConfigurationBase<PasswordResetToken, int>
    {
        public PasswordResetTokenMapping()
        {
            this.ToTable("PasswordResetToken");
            this.Property(e => e.UserId)
                .IsRequired();
            this.HasRequired(e => e.EMail)
                .WithMany()
                .HasForeignKey(l => l.EMailId);
        }
    }
}