using AS.Domain.Entities;
using AS.Infrastructure.Tests;
using System;
using System.Diagnostics;
using Xunit;

namespace AS.Services.Tests
{
    public class MailServiceTest : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        public MailServiceTest(TestFixture fixture)
        {
            this._fixture = fixture;
        }

        //This is actually bad testing
        [Theory]
        [InlineData(2000)]
        public void SendEmail_Should_Not_Exceed_TimeoutDuration(int timeout)
        {
            MailService service = new MailService(null, null, null, null, null);
            EMail testMail = new EMail();
            testMail.SmtpClientTimeOut = timeout;
            testMail.SmtpPort = 587;
            testMail.SmtpEnableSsl = true;
            testMail.SmtpUserName = Faker.Internet.UserName();
            testMail.SmtpPassword = Faker.Internet.UserName();
            testMail.SmtpHostAddress = "smtp.gmail.com";
            testMail.FromAddress = Faker.Internet.Email();
            testMail.FromName = Faker.Internet.UserName();
            testMail.Body = Faker.Lorem.Sentence();
            testMail.Subject = Faker.Lorem.Sentence();
            testMail.Receivers = Faker.Internet.Email();

            Stopwatch sw = Stopwatch.StartNew();
            Assert.Throws<AggregateException>(() => service.SendEMail(testMail));
            sw.Stop();
            Assert.True(sw.ElapsedMilliseconds - timeout < 1000);
        }
    }
}