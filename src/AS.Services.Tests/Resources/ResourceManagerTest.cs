using AS.Domain.Entities;
using AS.Domain.Interfaces;
using AS.Services.Interfaces;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AS.Services.Tests
{
    public class ResourceManagerTest
    {
        private readonly List<StringResource> resourceList;

        public ResourceManagerTest()
        {
            //Test Data
            resourceList = new List<StringResource>();
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Activities",
                Value = "Activities"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Activity_Activity",
                Value = "Activity"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Activity_Time",
                Value = "Activity Time"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Admin_EMailSettingMissingNotification",
                Value = "There is no  e-mail setting found . Please add one . Otherwise e-mails will not be send."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Admin_IPQueryApiKeyMissing",
                Value = "IPInfoDbApiKey  value is missing in config AppSettings. Client country info will not be provided until this key is provided."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Admin_MenuTitle",
                Value = "Main Menu"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Application_Settings",
                Value = "Application Settings"
            });
        }

        [Fact]
        public void GetString_Shall_Return_Corret_Values()
        {
            Mock<IContextProvider> contextProvider = new Mock<IContextProvider>();
            contextProvider.Setup(m => m.LanguageCode).Returns("en-US");
            Mock<IResourceService> resourceService = new Mock<IResourceService>();
            resourceService.Setup(m => m.FetchAll()).Returns(resourceList);

            IResourceManager resourceMan = new ResourceManager(contextProvider.Object,
                resourceService.Object);
            Assert.Equal(resourceMan.GetString("Admin_MenuTitle"), "Main Menu");
            Assert.Equal(resourceMan.GetString("Application_Settings"), "Application Settings");
            Assert.Equal(resourceMan.GetString("DummyTest"), "DummyTest");
            Assert.Equal(resourceMan.GetStringResources().Count(), resourceList.Count());
        }

        [Fact]
        public void GetStringResource_Should_Return_Correct_Amount_Of_Items()
        {
            Mock<IContextProvider> contextProvider = new Mock<IContextProvider>();
            contextProvider.Setup(m => m.LanguageCode).Returns("en-US");
            Mock<IResourceService> resourceService = new Mock<IResourceService>();
            resourceService.Setup(m => m.FetchAll()).Returns(resourceList);

            IResourceManager resourceMan = new ResourceManager(contextProvider.Object,
                resourceService.Object);

            Assert.Equal(resourceMan.GetStringResources().Count(), resourceList.Count());
            resourceMan.ClearCache();
            resourceList.Last().CultureCode = "tr-TR";
            Assert.Equal(resourceMan.GetStringResources().Count(), resourceList.Count() - 1);
        }
    }
}