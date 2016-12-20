using AS.Domain.Interfaces;
using AS.Infrastructure.Validation;
using Moq;
using System.Linq;
using Xunit;

namespace AS.Infrastructure.Tests.Validation
{
    public class EmailValidatorTest
    {
        [Theory]
        [InlineData("admin@test.com")]
        [InlineData("tester1234@test.edu")]
        [InlineData("firstname+lastname@example.com")]
        [InlineData("firstname-lastname@example.com")]
        [InlineData("email@example.co.jp")]
        [InlineData("_______@example.com")]
        [InlineData("1234567890@example.com")]
        [InlineData("email@example.museum")]
        public void EmailValidator_Should_Return_Valid(string email)
        {
            Mock<IResourceManager> mockResourceManager = new Mock<IResourceManager>();
            mockResourceManager.Setup(m => m.GetString(It.IsAny<string>())).Returns(string.Empty);
            EmailAddressValidator validator = new EmailAddressValidator(mockResourceManager.Object);

            Assert.True(validator.Validate(email).Succeeded);
        }

        [Theory]
        [InlineData("plainaddress")]
        [InlineData("#@%^%#$@#$@#.com")]
        [InlineData("@example.com")]
        [InlineData("email..email@example.com")]
        [InlineData("email@example..com")]
        [InlineData("email@111.222.333.44444")]
        [InlineData("Abc..123@example.com")]
        [InlineData("email@-example.com")]
        [InlineData("email@example@example.com")]
        public void EmailValidator_Should_Return_Invalid(string email)
        {
            Mock<IResourceManager> mockResourceManager = new Mock<IResourceManager>();
            mockResourceManager.Setup(m => m.GetString(It.IsAny<string>())).Returns("Email Address Error");
            EmailAddressValidator validator = new EmailAddressValidator(mockResourceManager.Object);

            Assert.False(validator.Validate(email).Succeeded);
            Assert.True(validator.Validate(email).Errors.Contains("Email Address Error"));
        }
    }
}