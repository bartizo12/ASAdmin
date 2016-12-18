using AS.Domain.Entities;
using System.Linq;
using Xunit;

namespace AS.Services.Tests
{
    public class StringResourceDictionaryTest
    {
        [Theory]
        [InlineData("en-US", "Customer_FirstName", "John")]
        public void GetString_Should_Return_ResourceValue_If_String_Exists(string cultureCode, string name, string value)
        {
            StringResourceDictionary dict = new StringResourceDictionary();
            dict.Add(new StringResource()
            {
                CultureCode = cultureCode,
                Name = name,
                Value = value
            });
            string returnValue = dict.GetString(cultureCode, name);
            Assert.Equal(returnValue, value);
        }

        [Theory]
        [InlineData("Customer_LastName")]
        [InlineData(null)]
        [InlineData("")]
        public void GetString_Should_Return_ResourceName_When_Variables_Are_Invalid(string resourceName)
        {
            StringResourceDictionary dict = new StringResourceDictionary();
            dict.Add(new StringResource()
            {
                CultureCode = "en-US",
                Name = "Customer_FirstName",
                Value = "FirstName"
            });
            Assert.Equal(resourceName, dict.GetString(null, resourceName));
            Assert.Equal(resourceName, dict.GetString("tr-TR", resourceName));
            Assert.Equal(resourceName, dict.GetString(string.Empty, resourceName));
            Assert.Equal(resourceName, dict.GetString("en-US", resourceName));
            Assert.Equal(resourceName, dict.GetString("en-US", resourceName));
            Assert.NotEqual("Customer_FirstName", dict.GetString("en-US", "Customer_FirstName"));
        }

        [Fact]
        public void GetAll_Should_Return_Correct_Values()
        {
            StringResourceDictionary dict = new StringResourceDictionary();
            dict.Add(new StringResource()
            {
                CultureCode = "en-US",
                Name = "Customer_FirstName",
                Value = "FirstName"
            });
            dict.Add(new StringResource()
            {
                CultureCode = "en-US",
                Name = "Customer_LastName",
                Value = "LastName"
            });
            dict.Add(new StringResource()
            {
                CultureCode = "en-US",
                Name = "Customer_FirstName",
                Value = "FirstName"
            });
            dict.Add(new StringResource()
            {
                CultureCode = "tr-TR",
                Name = "Customer_FirstName",
                Value = "Ad"
            });
            Assert.NotEmpty(dict.GetAll("en-US"));
            Assert.NotEmpty(dict.GetAll("tr-TR"));
            Assert.Empty(dict.GetAll("ru-RU"));
            Assert.Equal(dict.GetAll("en-US").Count(), 2);
        }
    }
}