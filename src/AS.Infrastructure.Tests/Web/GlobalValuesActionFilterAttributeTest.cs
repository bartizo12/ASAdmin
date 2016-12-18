using AS.Domain.Entities;
using AS.Domain.Interfaces;
using AS.Domain.Settings;
using AS.Infrastructure.Web.Mvc;
using AS.Infrastructure.Web.Mvc.Filters;
using Faker;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Xunit;

namespace AS.Infrastructure.Tests.Web
{
    public class GlobalValuesActionFilterAttributeTest
    {
        internal class DummyModel : ASModelBase { }

        internal class DummyController : ControllerBase
        {
            public new ViewDataDictionary ViewData
            {
                get; set;
            }

            protected override void ExecuteCore()
            {
            }
        }

        [Fact]
        public void GlobalValuesActionFilterAttribute_Should_Fill_ModelHeader_With_CorrectValues()
        {
            var resourceList = new List<StringResource>();

            // Generate Test Data
            Random random = new Random();
            for (int i = 0; i < 200; i++)
            {
                resourceList.Add(new StringResource()
                {
                    AvailableOnClientSide = random.Next() % 2 == 0,
                    Name = Lorem.Sentence(),
                    Value = Lorem.Sentence(),
                    CultureCode = "en-US"
                });
            }

            DummyModel model = new DummyModel();
            Mock<IResourceManager> resMan = new Mock<IResourceManager>();
            Mock<ISettingContainer<AppSetting>> appSettings = new Mock<ISettingContainer<AppSetting>>();
            Mock<IContextProvider> contextProvider = new Mock<IContextProvider>();
            ResultExecutingContext context = new ResultExecutingContext();
            context.Controller = new DummyController();
            context.Controller.ViewData = new ViewDataDictionary()
            {
                Model = model
            };
            resMan.Setup(m => m.GetStringResources()).Returns(resourceList);
            resMan.Setup(m => m.GetAvailableCultureList()).Returns(new List<string>() { "en-US" });
            appSettings.Setup(m => m.Contains(It.IsAny<string>())).Returns(false);
            contextProvider.Setup(m => m.LanguageCode).Returns("en-US");

            Mock<ISettingManager> settingManagerMock = new Mock<ISettingManager>();
            settingManagerMock.Setup(m => m.GetContainer<AppSetting>()).Returns(appSettings.Object);

            GlobalValuesActionFilterAttribute attribute = new GlobalValuesActionFilterAttribute(settingManagerMock.Object,
              contextProvider.Object, resMan.Object);
            attribute.OnResultExecuting(context);
            Assert.Equal(model.Header.ClientResources.Count, resourceList.Count(rs => rs.AvailableOnClientSide == true));
            var testKey = model.Header.ClientResources.First().Key;
            var testVal = model.Header.ClientResources.First().Value;
            Assert.True(resourceList.Any(rs => rs.Name == testKey));
            Assert.True(resourceList.Any(rs => rs.Value == testVal));
        }
    }
}