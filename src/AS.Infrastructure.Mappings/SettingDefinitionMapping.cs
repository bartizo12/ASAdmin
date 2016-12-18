using AS.Domain.Entities;
using AS.Infrastructure.Data.EF;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace AS.Infrastructure.Mappings
{
    public class SettingDefinitionMapping : EntityTypeConfigurationBase<SettingDefinition, int>
    {
        public SettingDefinitionMapping()
        {
            this.ToTable("SettingDefinition");
            this.Property(l => l.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_UNIQUE_Setting_DEF_NAME", 1) { IsUnique = true }));

            this.Property(l => l.Description)
                .IsRequired()
                .HasMaxLength(1000);
            this.Property(l => l.Field1)
                .HasMaxLength(100);
            this.Property(l => l.Field2)
                .HasMaxLength(100);
            this.Property(l => l.Field3)
                .HasMaxLength(100);
            this.Property(l => l.Field4)
                .HasMaxLength(100);
            this.Property(l => l.Field5)
                .HasMaxLength(100);
            this.Property(l => l.Field6)
                .HasMaxLength(100);
            this.Property(l => l.Field7)
                .HasMaxLength(100);
            this.Property(l => l.Field8)
                .HasMaxLength(100);
            this.Property(l => l.Field9)
                .HasMaxLength(100);
            this.Property(l => l.Field10)
                .HasMaxLength(100);
            this.Property(l => l.Field11)
                .HasMaxLength(100);
            this.Property(l => l.Field12)
                .HasMaxLength(100);
            this.Property(l => l.Field13)
                .HasMaxLength(100);
            this.Property(l => l.Field14)
                .HasMaxLength(100);
            this.Property(l => l.Field15)
                .HasMaxLength(100);
        }
    }
}