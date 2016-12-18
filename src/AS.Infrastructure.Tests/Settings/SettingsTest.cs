using AS.Domain.Entities;
using AS.Domain.Settings;
using System.Collections.Generic;
using Xunit;

namespace AS.Infrastructure.Tests
{
    public class SettingsTest
    {
        [Fact]
        public void Load_Shall_Load_Settings_Correctly()
        {
            AppSettingContainer settingContainer = new AppSettingContainer();
            settingContainer.Load(this.GenerateSettingValues());
            Assert.NotNull(settingContainer.Default);
            Assert.Equal(settingContainer.SettingName, "AppSetting");
            Assert.Equal(settingContainer["Test"].Value, "Test");
        }

        private IEnumerable<SettingValue> GenerateSettingValues()
        {
            List<SettingValue> values = new List<SettingValue>();
            SettingDefinition appSettingDef = new SettingDefinition()
            {
                Id = 1,
                Name = "AppSetting",
                Description = "Application Settings",
                Field1 = "Name",
                Field2 = "Value",
                Field3 = "Comment",
                FieldRequired1 = true,
                FieldRequired2 = true,
                FieldRequired3 = false
            };
            values.Add(new SettingValue()
            {
                Id = 999,
                Field1 = "Test",
                Field2 = "Test",
                Field3 = "Test",
                SettingDefinitionID = appSettingDef.Id,
                SettingDefinition = appSettingDef
            });
            for (int i = 0; i < 100; i++)
            {
                values.Add(new SettingValue()
                {
                    Id = i + 1,
                    Field1 = Faker.Lorem.Sentence() + i.ToString(),
                    Field2 = Faker.Lorem.Sentence(),
                    Field3 = Faker.Lorem.Sentence(),
                    SettingDefinitionID = appSettingDef.Id,
                    SettingDefinition = appSettingDef
                });
            }

            return values;
        }
    }
}