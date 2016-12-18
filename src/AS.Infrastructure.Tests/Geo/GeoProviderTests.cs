using AS.Domain.Interfaces;
using AS.Domain.Settings;
using Moq;
using Xunit;

namespace AS.Infrastructure.Tests.Geo
{
    public class GeoProviderTests
    {
        [Theory]
        [InlineData("31.13.92.36")]
        [InlineData("192.30.253.113")]
        public void GeoProvider_GetCountryInfo_ShouldThrowError_OnInvalidAPIKey(string ipAddress)
        {
            Mock<ISettingContainer<AppSetting>> appsettingMock = new Mock<ISettingContainer<AppSetting>>();
            Mock<ISettingContainer<UrlAddress>> urlAddressesMock = new Mock<ISettingContainer<UrlAddress>>();

            urlAddressesMock.Setup(m => m.Contains("IPCountryQueryUrl")).Returns(true);
            urlAddressesMock.SetupGet(m => m["IPCountryQueryUrl"])
                            .Returns(new UrlAddress()
                                    {
                                        Name = "IPCountryQueryUrl",
                                        Address = "http://api.ipinfodb.com/v3/ip-country/?key={{apiKey}}&amp;ip={{ip}}"
                                    });
            appsettingMock.Setup(m => m.Contains("IPInfoDbApiKey")).Returns(true);
            appsettingMock.SetupGet(m => m["IPInfoDbApiKey"])
                          .Returns(new AppSetting()
                                    {
                                        Name = "IPInfoDbApiKey",
                                        Value = "InvalidKey"
                                    });

            Mock<ISettingManager> settingManagerMock = new Mock<ISettingManager>();
            settingManagerMock.Setup(m => m.GetContainer<AppSetting>()).Returns(appsettingMock.Object);
            settingManagerMock.Setup(m => m.GetContainer<UrlAddress>()).Returns(urlAddressesMock.Object);
            IGeoProvider geoProvider = new GeoProvider(settingManagerMock.Object);
            Assert.Throws<ASException>(() => geoProvider.GetCountryInfo(ipAddress));
        }
    }
}