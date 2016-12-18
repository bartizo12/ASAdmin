using AS.Infrastructure.Tests;
using System.Collections.Generic;
using Xunit;

namespace AS.Services.Tests
{
    public class NotificationServiceTest : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        public NotificationServiceTest(TestFixture fixture)
        {
            this._fixture = fixture;
        }

        [Fact]
        public void UpdateAsSeen_Should_Update_Notifications()
        {
            NotificationService service = new NotificationService(this._fixture.DbContext);
            List<int> currentIdList = new List<int>();
            currentIdList.Add(service.Insert(1, "Test", "1").Id);
            currentIdList.Add(service.Insert(1, "Test", "2").Id);
            currentIdList.Add(service.Insert(1, "Test", "3").Id);
            currentIdList.Add(service.Insert(1, "Test", "4").Id);
            currentIdList.Add(service.Insert(1, "Test", "5").Id);
            currentIdList.Add(service.Insert(1, "Test", "6").Id);
            service.Insert(1, "Test", "7");
            service.Insert(1, "Test", "8");
            service.Insert(1, "Test", "9");
            Assert.Equal(service.GetUnseenCount(1), 9);
            Assert.Equal(service.GetNotifications(1, 5).Count, 5);
            service.UpdateAsSeen(currentIdList);
            Assert.Equal(service.GetUnseenCount(1), 3);
        }
    }
}