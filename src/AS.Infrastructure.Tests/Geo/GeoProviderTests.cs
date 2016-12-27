using AS.Domain.Interfaces;
using AS.Domain.Settings;
using Moq;
using Xunit;

namespace AS.Infrastructure.Tests.Geo
{
    public class GeoProviderTests
    {
        [Theory]
        [InlineData("31.13.92.36", "NL")]
        [InlineData("192.30.253.113", "US")]
        public void GeoProvider_GetCountryInfo_ShouldThrowError_OnInvalidAPIKey(string ipAddress,string country)
        {
            Mock<ISettingContainer<UrlAddress>> urlAddressesMock = new Mock<ISettingContainer<UrlAddress>>();

            urlAddressesMock.Setup(m => m.Contains("IPCountryQueryUrl")).Returns(true);
            urlAddressesMock.SetupGet(m => m["IPCountryQueryUrl"])
                            .Returns(new UrlAddress()
                            {
                                Name = "IPCountryQueryUrl",
                                Address = "http://ip-api.com/json/{{ip}}"
                            });

            Mock<ISettingManager> settingManagerMock = new Mock<ISettingManager>();
            settingManagerMock.Setup(m => m.GetContainer<UrlAddress>()).Returns(urlAddressesMock.Object);
            IGeoProvider geoProvider = new GeoProvider(settingManagerMock.Object);
            Assert.Equal(country, geoProvider.GetCountryInfo(ipAddress).Key);
        }
    }
}