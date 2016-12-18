using AS.Domain.Entities;
using AS.Domain.Interfaces;
using AS.Infrastructure.Reflection;
using System;
using Xunit;

namespace AS.Infrastructure.Tests
{
    public class TypeFinderTest
    {
        private interface TestInterface { }

        private abstract class TestClass { }

        [Theory]
        [InlineData(typeof(SettingValue))]
        [InlineData(typeof(SettingDefinition))]
        [InlineData(typeof(EMail))]
        public void FindClassesOfType_Should_Return_NonEmpty_List(Type type)
        {
            ITypeFinder typeFinder = new TypeFinder();
            Assert.NotEmpty(typeFinder.FindClassesOfType(type, true));
        }

        [Theory]
        [InlineData(typeof(TestInterface))]
        [InlineData(typeof(TestClass))]
        public void FindClassesOfType_Should_Return_Empty_List(Type type)
        {
            ITypeFinder typeFinder = new TypeFinder();
            Assert.Empty(typeFinder.FindClassesOfType(type, true));
        }
    }
}