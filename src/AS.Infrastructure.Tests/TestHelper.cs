using Faker;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AS.Infrastructure.Tests
{
    internal static class TestHelper
    {
        private static readonly Random random = new Random();

        /// <summary>
        /// Generates random usernames
        /// </summary>
        /// <param name="count">Total number of username to be generated</param>
        /// <returns>Generated usernames</returns>
        public static IEnumerable<string> GenerateUsernameList(int count)
        {
            return Enumerable.Repeat(Internet.UserName(), count);
        }

        public static long RandomLong()
        {
            return random.RandomLong();
        }

        public static int Random(int maxValue)
        {
            return random.Next(maxValue);
        }
    }
}