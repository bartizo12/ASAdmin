using AS.Domain.Interfaces;
using AS.Domain.Settings;
using AS.Infrastructure.Validation;
using Moq;
using Xunit;

namespace AS.Infrastructure.Tests
{
    public class PasswordValidatorTest
    {
        [Theory]
        [InlineData("1234")]
        [InlineData("abc123ABC")]
        [InlineData("**_~,.123!'^+%/()=?_")]
        public void PasswordValidator_Should_Return_Valid(string password)
        {
            Mock<ISettingManager> mockSettingManager = new Mock<ISettingManager>();
            Mock<IResourceManager> mockResourceManager = new Mock<IResourceManager>();
            Mock<ISettingContainer<MembershipSetting>> mockMembershipSettingContainer = new Mock<ISettingContainer<MembershipSetting>>();

            mockMembershipSettingContainer.SetupGet(m => m.Default)
                                           .Returns(new MembershipSetting()
                                           {
                                               RequireDigitInPassword = false,
                                               MinimumPasswordRequiredLength = 4,
                                               RequireNonLetterOrDigitInPassword = false,
                                               RequireUppercaseInPassword = false
                                           });
            mockSettingManager.Setup(m => m.GetContainer<MembershipSetting>()).Returns(mockMembershipSettingContainer.Object);
            mockResourceManager.Setup(m => m.GetString(It.IsAny<string>())).Returns(string.Empty);

            PasswordValidator validator = new PasswordValidator(mockSettingManager.Object, mockResourceManager.Object);

            Assert.True(validator.Validate(password).Succeeded);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("123")]
        [InlineData("abc defg")]
        [InlineData("abcdefg ")]
        public void PasswordValidator_Should_Return_Invalid(string password)
        {
            Mock<ISettingManager> mockSettingManager = new Mock<ISettingManager>();
            Mock<IResourceManager> mockResourceManager = new Mock<IResourceManager>();
            Mock<ISettingContainer<MembershipSetting>> mockMembershipSettingContainer = new Mock<ISettingContainer<MembershipSetting>>();

            mockMembershipSettingContainer.SetupGet(m => m.Default)
                                           .Returns(new MembershipSetting()
                                           {
                                               RequireDigitInPassword = false,
                                               MinimumPasswordRequiredLength = 4,
                                               RequireNonLetterOrDigitInPassword = false,
                                               RequireUppercaseInPassword = false
                                           });
            mockSettingManager.Setup(m => m.GetContainer<MembershipSetting>()).Returns(mockMembershipSettingContainer.Object);
            mockResourceManager.Setup(m => m.GetString(It.IsAny<string>())).Returns(string.Empty);

            PasswordValidator validator = new PasswordValidator(mockSettingManager.Object, mockResourceManager.Object);

            Assert.False(validator.Validate(password).Succeeded);
        }
    }
}