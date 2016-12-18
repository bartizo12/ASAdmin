using AS.Domain.Entities;
using AS.Domain.Interfaces;
using System;
using Xunit;

namespace AS.Infrastructure.Tests.Extensions
{
    public class StringExtensionsTest
    {
        [Fact]
        public void ToEnum_Should_Convert_Correctly()
        {
            Assert.Equal("Warn".ToEnum(LogLevel.Debug), LogLevel.Warn);
            Assert.Equal("Running".ToEnum(JobStatus.Queued), JobStatus.Running);
            Assert.Equal("InvalidEnum".ToEnum(JobStatus.Queued), JobStatus.Queued);
        }

        [Fact]
        public void ToEnum_Should_Throw_If_GenericType_IsNot_Enum()
        {
            Assert.Throws<ArgumentException>(() => "Test".ToEnum(0));
        }
    }
}