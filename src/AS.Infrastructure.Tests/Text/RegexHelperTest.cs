using Xunit;

namespace AS.Infrastructure.Tests
{
    public class RegexHelperTest
    {
        [Theory]
        [InlineData("test@testSmtp.com", "t**t@testSmtp.com")]
        [InlineData("test@testSmtp.com;nazmi@testSmtp.com", "t**t@testSmtp.com;n***i@testSmtp.com")]
        [InlineData("test@testSmtp.com;nazmi@testSmtp.com;info@gmail.com;", "t**t@testSmtp.com;n***i@testSmtp.com;i**o@gmail.com;")]
        public void MaskEmail_Should_Mask_Correctly(string email, string maskedEmail)
        {
            Assert.Equal(RegexHelper.MaskEmailAddress(email), maskedEmail);
        }

        [Theory]
        [InlineData("Test", "****")]
        [InlineData("password", "********")]
        [InlineData("", "")]
        [InlineData(null, null)]
        public void Mask_Should_Mask_Correctly(string input, string result)
        {
            Assert.Equal(RegexHelper.Mask(input), result);
        }
    }
}