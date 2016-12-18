using AS.Domain.Entities;
using AS.Infrastructure.Data.EF;

namespace AS.Infrastructure.Mappings
{
    public partial class ContactUsMapping : EntityTypeConfigurationBase<ContactUs, int>
    {
        public ContactUsMapping()
        {
            this.ToTable("ContactUs");
            this.Property(l => l.Country)
                .HasMaxLength(80);
            this.Property(l => l.EmailAddress)
                .HasMaxLength(255)
                .IsRequired();
            this.Property(l => l.FullName)
                .HasMaxLength(100)
                .IsRequired();
            this.Property(l => l.IPAddress)
                .HasMaxLength(20)
                .IsRequired();
            this.Property(l => l.Message)
                .IsRequired()
                .HasMaxLength(1000);
            this.Property(l => l.Subject)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}