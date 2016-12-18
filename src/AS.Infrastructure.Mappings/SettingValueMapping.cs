using AS.Domain.Entities;
using AS.Infrastructure.Data.EF;

namespace AS.Infrastructure.Mappings
{
    public class SettingValueMapping : EntityTypeConfigurationBase<SettingValue, int>
    {
        public SettingValueMapping()
        {
            this.ToTable("SettingValue");
            this.HasRequired(l => l.SettingDefinition)
                .WithMany(s => s.SettingValues)
                .HasForeignKey(l => l.SettingDefinitionID);
        }
    }
}