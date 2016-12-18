using AS.Domain.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace AS.Infrastructure
{
    /// <summary>
    /// Encryption class that uses AES algorithm
    /// Note that ; IV is generated at the encryption stage and prepend to encrypted string
    /// and at decryption stage it is extracted and used again.
    /// </summary>
    public class AESEncryptionProvider : IEncryptionProvider
    {
        /// <summary>
        /// Generates a valid key to be used  in AES encryption
        /// </summary>
        /// <returns></returns>
        public string GenerateKey()
        {
            string key = string.Empty;

            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.GenerateKey();
                key = Convert.ToBase64String(aes.Key);
            }
            return key;
        }

        /// <summary>
        /// Encrypts plainText by using AES algorithm
        /// </summary>
        /// <param name="plainText">Text to be encrypted</param>
        /// <param name="key">Key to be used</param>
        /// <returns>Encrypted Text</returns>
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public string Encrypt(string plainText, string key)
        {
            string encrypted = string.Empty;

            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException("Invalid input.", "plainText");

            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Padding = PaddingMode.Zeros;
                aes.GenerateIV();
                aes.Key = Convert.FromBase64String(key);
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        byte[] plainTextBytes = plainText.ToCharArray().Select(c => (byte)c).ToArray();
                        csEncrypt.Write(aes.IV, 0, aes.IV.Length);
                        csEncrypt.Write(plainTextBytes, 0, plainTextBytes.Length);
                        csEncrypt.FlushFinalBlock();
                        encrypted = new string(msEncrypt.ToArray().Select(b => (char)b).ToArray());
                    }
                }
            }

            return encrypted;
        }

        /// <summary>
        /// Decrypts cipherText into plaintext by using AES algorithm
        /// </summary>
        /// <param name="cipherText">Text to be decrypted</param>
        /// <param name="key">Key</param>
        /// <returns>Plain text</returns>
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public string Decrypt(string cipherText, string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentException("Invalid input.", "cipherText");

            string plaintext = string.Empty;
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Padding = PaddingMode.Zeros;
                aes.Key = Convert.FromBase64String(key);
                byte[] cipherTextBytes = cipherText.ToCharArray().Select(c => (byte)c).ToArray();
                aes.IV = cipherTextBytes.Take(16).ToArray();
                cipherTextBytes = cipherTextBytes.Skip(16).ToArray();

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherTextBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext.Trim('\0');
        }
    }
}