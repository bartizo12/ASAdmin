using AS.Domain.Interfaces;
using AS.Domain.Settings;
using AS.Infrastructure.Validation;
using Moq;
using System.Linq;
using Xunit;

namespace AS.Infrastructure.Tests
{
    public class UserNameValidatorTest
    {
        [Theory]
        [InlineData("admin")]
        [InlineData("mete")]
        [InlineData("kagan.test-p_@yy+")]
        [InlineData("1a-Q")]
        [InlineData(".1_2")]
        [InlineData(".1-2")]
        public void UserNameValidate_Should_Return_Valid(string userName)
        {
            Mock<ISettingManager> mockSettingManager = new Mock<ISettingManager>();
            Mock<IResourceManager> mockResourceManager = new Mock<IResourceManager>();
            Mock<ISettingContainer<MembershipSetting>> mockMembershipSettingContainer = new Mock<ISettingContainer<MembershipSetting>>();

            mockMembershipSettingContainer.SetupGet(m => m.Default)
                                           .Returns(new MembershipSetting()
                                           {
                                               AllowOnlyAlphanumericUserNames = false
                                           });
            mockSettingManager.Setup(m => m.GetContainer<MembershipSetting>()).Returns(mockMembershipSettingContainer.Object);
            mockResourceManager.Setup(m => m.GetString(It.IsAny<string>())).Returns(string.Empty);

            UsernameValidator validator = new UsernameValidator(mockSettingManager.Object, mockResourceManager.Object);
            Assert.True(validator.Validate(userName).Succeeded);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void UserNameValidate_Should_Return_CannotBeEmpty(string userName)
        {
            Mock<ISettingManager> mockSettingManager = new Mock<ISettingManager>();
            Mock<IResourceManager> mockResourceManager = new Mock<IResourceManager>();
            Mock<ISettingContainer<MembershipSetting>> mockMembershipSettingContainer = new Mock<ISettingContainer<MembershipSetting>>();

            mockMembershipSettingContainer.SetupGet(m => m.Default)
                                           .Returns(new MembershipSetting()
                                           {
                                               AllowOnlyAlphanumericUserNames = false
                                           });
            mockSettingManager.Setup(m => m.GetContainer<MembershipSetting>()).Returns(mockMembershipSettingContainer.Object);
            mockResourceManager.Setup(m => m.GetString(It.IsAny<string>())).Returns<string>(x => x);

            UsernameValidator validator = new UsernameValidator(mockSettingManager.Object, mockResourceManager.Object);

            Assert.False(validator.Validate(userName).Succeeded);
            Assert.Equal(validator.Validate(userName).Errors.First(), "UsernameCannotBeEmpty");
        }

        [Theory]
        [InlineData("joe")]
        [InlineData("j12")]
        [InlineData("Hu")]
        public void UserNameValidate_Should_Return_LengthMustBeInRange(string userName)
        {
            Mock<ISettingManager> mockSettingManager = new Mock<ISettingManager>();
            Mock<IResourceManager> mockResourceManager = new Mock<IResourceManager>();
            Mock<ISettingContainer<MembershipSetting>> mockMembershipSettingContainer = new Mock<ISettingContainer<MembershipSetting>>();

            mockMembershipSettingContainer.SetupGet(m => m.Default)
                                           .Returns(new MembershipSetting()
                                           {
                                               AllowOnlyAlphanumericUserNames = false
                                           });
            mockSettingManager.Setup(m => m.GetContainer<MembershipSetting>()).Returns(mockMembershipSettingContainer.Object);
            mockResourceManager.Setup(m => m.GetString(It.IsAny<string>())).Returns<string>(x => x);

            UsernameValidator validator = new UsernameValidator(mockSettingManager.Object, mockResourceManager.Object);
            Assert.False(validator.Validate(userName).Succeeded);
            Assert.Equal(validator.Validate(userName).Errors.First(), "UsernameLengthMustBeInRange");
        }

        [Theory]
        [InlineData("admin 123")]
        [InlineData("admin123!'^++&%/()")]
        [InlineData("123&admin")]
        [InlineData("1*2*3*4*5")]
        [InlineData("admin&")]
        [InlineData("<ad>min=")]
        public void UserNameValidate_Should_Return_CanOnlyCertainCharacters(string userName)
        {
            Mock<ISettingManager> mockSettingManager = new Mock<ISettingManager>();
            Mock<IResourceManager> mockResourceManager = new Mock<IResourceManager>();
            Mock<ISettingContainer<MembershipSetting>> mockMembershipSettingContainer = new Mock<ISettingContainer<MembershipSetting>>();

            mockMembershipSettingContainer.SetupGet(m => m.Default)
                                           .Returns(new MembershipSetting()
                                           {
                                               AllowOnlyAlphanumericUserNames = false,
                                           });
            mockSettingManager.Setup(m => m.GetContainer<MembershipSetting>()).Returns(mockMembershipSettingContainer.Object);
            mockResourceManager.Setup(m => m.GetString(It.IsAny<string>())).Returns<string>(x => x);

            UsernameValidator validator = new UsernameValidator(mockSettingManager.Object, mockResourceManager.Object);
            Assert.False(validator.Validate(userName).Succeeded);
            Assert.Equal(validator.Validate(userName).Errors.First(), "InvalidUsername");
        }
    }
}