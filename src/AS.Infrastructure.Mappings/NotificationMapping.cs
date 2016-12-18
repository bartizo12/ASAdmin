using AS.Domain.Entities;
using AS.Infrastructure.Data.EF;

namespace AS.Infrastructure.Mappings
{
    public class NotificationMapping : EntityTypeConfigurationBase<Notification, int>
    {
        public NotificationMapping()
        {
            this.ToTable("Notification");
            this.Property(e => e.UserId)
               .IsRequired();
            this.Property(e => e.Message)
                .IsRequired()
                .HasMaxLength(2000);
        }
    }
}