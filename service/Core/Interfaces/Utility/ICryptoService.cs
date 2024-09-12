namespace service.Core.Interfaces.Utility
{
    /// <summary>
    /// Provides an interface for cryptographic operations, primarily password decryption.
    /// </summary>
    public interface ICryptoService
    {
        /// <summary>
        /// Decrypts an encrypted password using a provided private key.
        /// </summary>
        /// <param name="encryptedPassword">The encrypted password to be decrypted.</param>
        /// <param name="privateKey">The private key used for decryption.</param>
        /// <returns>The decrypted password as a string.</returns>
        string DecryptPassword(string encryptedPassword, string privateKey);

        /// <summary>
        /// Encrypts a plaintext string using AES encryption.
        /// </summary>
        /// <param name="plainText">The plaintext string to be encrypted.</param>
        /// <param name="key">The encryption key. If null, a default key will be used.</param>
        /// <returns>The encrypted ciphertext as a string.</returns>
        Task<string> AesEncryptionAsync(string plainText, byte[]? key = null);

        /// <summary>
        /// Decrypts an AES-encrypted ciphertext string.
        /// </summary>
        /// <param name="cipherText">The ciphertext string to be decrypted.</param>
        /// <param name="key">The decryption key. If null, a default key will be used.</param>
        /// <returns>The decrypted plaintext as a string.</returns>
        Task<string> AesDecryptionAsync(string cipherText, byte[]? key = null);
    }
}
