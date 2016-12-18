using AS.Domain.Entities;
using AS.Infrastructure.Data.EF;

namespace AS.Infrastructure.Mappings
{
    public class RequestLogMapping : EntityTypeConfigurationBase<RequestLog, long>
    {
        public RequestLogMapping()
        {
            this.ToTable("RequestLog");
            this.Property(l => l.AbsolutePath)
                .HasMaxLength(300)
                .IsRequired();
            this.Property(l => l.BrowserType)
                .HasMaxLength(50)
                .IsRequired();
            this.Property(l => l.ClientIP)
                .HasMaxLength(20)
                .IsRequired();
            this.Property(l => l.CountryCode)
                .HasMaxLength(4);
            this.Property(l => l.HttpMethod)
                .HasMaxLength(10)
                .IsRequired();
            this.Property(l => l.SessionID)
                .HasMaxLength(100);
            this.Property(l => l.UserAgent)
                .HasMaxLength(500);
        }
    }
}