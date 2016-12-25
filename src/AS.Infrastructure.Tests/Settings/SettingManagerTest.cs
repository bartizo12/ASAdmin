using AS.Domain.Entities;
using AS.Domain.Settings;
using AS.Infrastructure.Reflection;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace AS.Infrastructure.Tests.Settings
{
    public class SettingManagerTest
    {
        [Fact]
        public void SettingManager_Should_Get_Container_Correctly_Loaded()
        {
            TypeFinder typeFinder = new TypeFinder();
            Mock<ISettingDataProvider> mockSettingDataProvider = new Mock<ISettingDataProvider>();
            mockSettingDataProvider.Setup(m => m.FetchSettingValues())
                .Returns(this.GenerateSettingValues());

            SettingManager manager = new SettingManager(mockSettingDataProvider.Object,typeFinder);
            var container = manager.GetContainer<AppSetting>();
            Assert.NotNull(container);
            Assert.True(container.Contains("Test"));
            Assert.True(container["Test"].Value == "Test");
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