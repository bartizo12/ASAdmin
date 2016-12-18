using AS.Domain.Interfaces;
using Faker;
using System;
using Xunit;

namespace AS.Infrastructure.Tests
{
    public class AESEncryptionProviderTest
    {
        [Fact]
        public void AESEncryptionProvider_Should_Generate_ValidKey()
        {
            IEncryptionProvider enc = new AESEncryptionProvider();
            string key = enc.GenerateKey();
            Assert.NotNull(key);
            Assert.True(key.Length > 0);
            //Key sizes	128, 192 or 256 bits
            Assert.True(Convert.FromBase64String(key).Length >= 128 / 8);
        }

        [Fact]
        public void AESEncryptionProvider_Should_Encrypt_Decrypt_Correctly()
        {
            IEncryptionProvider enc = new AESEncryptionProvider();

            for (int i = 0; i < 100; i++)
            {
                string plainText = Lorem.Paragraph(120);
                string key = enc.GenerateKey();
                string cipherText = enc.Encrypt(plainText, key);
                string decryptedText = enc.Decrypt(cipherText, key);

                Assert.Equal(plainText, decryptedText);
            }
        }
    }
}