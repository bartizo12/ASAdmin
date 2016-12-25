using Moq;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Threading;
using Xunit;

namespace AS.Infrastructure.Tests
{
    public class DbCommandInterceptorTest : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        public DbCommandInterceptorTest(TestFixture fixture)
        {
            this._fixture = fixture;
        }
        [Fact]
        public void Interceptor_Should_Log_Without_Throwing_Any_Exception()
        {
            Mock<DbCommand> mockNonQueryDbCmd = new Mock<DbCommand>();
            mockNonQueryDbCmd.SetupGet(p => p.CommandText).Returns("Unit Test CommandText");
            Mock<DbCommandInterceptionContext<int>> mockInterceptionContextInt = new Mock<DbCommandInterceptionContext<int>>();

            this._fixture.DbCommandInterceptor.NonQueryExecuting(mockNonQueryDbCmd.Object, mockInterceptionContextInt.Object);
            Thread.Sleep(TestHelper.Random(2000));
            this._fixture.DbCommandInterceptor.NonQueryExecuted(mockNonQueryDbCmd.Object, mockInterceptionContextInt.Object);
        }
    }
}