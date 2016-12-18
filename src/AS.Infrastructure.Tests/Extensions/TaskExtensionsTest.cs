using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AS.Infrastructure.Tests
{
    public class TaskExtensionsTest
    {
        [Theory]
        [InlineData(1000)]
        [InlineData(1200)]
        [InlineData(2000)]
        public void TimeoutAfter_Should_Throw_When_Task_Timesout(int timeOutDurationMsec)
        {
            Assert.Throws<AggregateException>(() => Task.Factory.StartNew(() => Thread.Sleep(100000))
                                                .TimeoutAfter(timeOutDurationMsec)
                                                .Wait());
        }

        [Theory]
        [InlineData(1000)]
        [InlineData(1200)]
        [InlineData(2000)]
        public void TimeoutAfter_Should_Not_Throw(int timeOutDurationMsec)
        {
            Task.Factory.StartNew(() => Thread.Sleep(990))
                .TimeoutAfter(timeOutDurationMsec)
                .Wait();
        }
    }
}