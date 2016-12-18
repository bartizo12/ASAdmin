using AS.Domain.Entities;
using AS.Infrastructure.Data.EF;

namespace AS.Infrastructure.Mappings
{
    public class JobDefinitionMapping : EntityTypeConfigurationBase<JobDefinition, int>
    {
        public JobDefinitionMapping()
        {
            this.ToTable("JobDefinition");
            this.Property(e => e.JobTypeName)
                .IsRequired()
                .HasMaxLength(300);
            this.Property(e => e.JobTypeName)
                .IsRequired()
                .HasMaxLength(1000);
            this.Property(e => e.RunInterval)
                .IsRequired();
            this.Property(e => e.LastExecutionTime)
                .IsOptional();
        }
    }
}