using AS.Domain.Interfaces;
using AS.Domain.Settings;
using AS.Jobs;
using Moq;
using System.Diagnostics;
using Xunit;

namespace AS.Services.Tests
{
    public class PingJobTest
    {
        //Note that ,this test fails if you have no connection to google
        [Fact]
        public void PingJob_ShallNot_Exceed_Timeout()
        {
            Mock<ILogger> mockLogger = new Mock<ILogger>();
            Mock<ISettingContainer<UrlAddress>> mockUrls = new Mock<ISettingContainer<UrlAddress>>();
            mockUrls.Setup(m => m.Contains("PingUrl")).Returns(true);
            mockUrls.SetupGet(m => m["PingUrl"]).Returns(new UrlAddress()
            {
                Address = "http://www.google.com"
            });

            Mock<ISettingManager> settingManagerMock = new Mock<ISettingManager>();
            settingManagerMock.Setup(m => m.GetContainer<UrlAddress>()).Returns(mockUrls.Object);

            PingJob job = new PingJob(mockLogger.Object, settingManagerMock.Object);
            Stopwatch sw = Stopwatch.StartNew();
            job.Execute();
            sw.Stop();
            Assert.True(sw.ElapsedMilliseconds < 6000);
        }
    }
}