using System;
using System.Collections.Generic;
using Xunit;

namespace AS.Infrastructure.Tests
{
    public class DateTimeExtensionTest
    {
        [Theory]
        [MemberData(nameof(DateTimeTestData))]
        public void ToJavaScriptMilliseconds_Should_Calculate_Correctly(DateTime dateTime, long expectedJsmsecs)
        {
            long difference = Math.Abs(dateTime.ToJavaScriptMilliseconds() - expectedJsmsecs);

            Assert.True(difference < 1000); // Cannot be more than 1 sec difference
        }

        public static IEnumerable<object[]> DateTimeTestData
        {
            get
            {
                return new[]
                {
                 new object[] { new DateTime(2016,11,14,9,14,02,DateTimeKind.Utc), 1479114842513},
                 new object[] { new DateTime(2013,09,13,23,27,05,DateTimeKind.Utc), 1379114825132}
                };
            }
        }
    }
}