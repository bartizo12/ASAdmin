namespace AS.Domain.Interfaces
{
    /// <summary>
    /// Symmetric Encryption provider to encrypt/decrypt strings.
    /// All symmetric encryption algorithms can implement this interface.
    /// </summary>
    public interface IEncryptionProvider
    {
        /// <summary>
        /// Generates a key to be used in the encryption
        /// </summary>
        /// <returns>Randomly generated key</returns>
        string GenerateKey();

        /// <summary>
        /// Encrypts plainText
        /// </summary>
        /// <param name="plainText">Text to be encrypted</param>
        /// <param name="key">Key to be used</param>
        /// <returns>Encrypted Text</returns>
        string Encrypt(string plainText, string key);

        /// <summary>
        /// Decrypts cipherText into plaintext
        /// </summary>
        /// <param name="cipherText">Text to be decrypted</param>
        /// <param name="key">Key</param>
        /// <returns>Plain text</returns>
        string Decrypt(string cipherText, string key);
    }
}