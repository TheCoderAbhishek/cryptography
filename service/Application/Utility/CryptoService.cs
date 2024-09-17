using service.Core.Interfaces.Utility;
using System.Security.Cryptography;
using System.Text;

namespace service.Application.Utility
{
    /// <summary>
    /// Provides an implementation of the ICryptoService interface for cryptographic operations, 
    /// focusing on password decryption using RSA.
    /// </summary>
    public class CryptoService(IConfiguration configuration) : ICryptoService
    {
        private readonly IConfiguration _configuration = configuration;

        #region Private Helper Methods

        /// <summary>
        /// Retrieves the AES encryption key from the configuration.
        /// </summary>
        /// <returns>The AES key as a byte array.</returns>
        private byte[] GetKeyFromConfig()
        {
            var key = _configuration["Aes:Key"];
            return Convert.FromBase64String(key!);
        }

        /// <summary>
        /// Retrieves the AES initialization vector (IV) from the configuration.
        /// </summary>
        /// <returns>The AES IV as a byte array.</returns>
        private byte[] GetIvFromConfig()
        {
            var iv = _configuration["Aes:Iv"];
            return Convert.FromBase64String(iv!);
        } 

        #endregion

        /// <summary>
        /// Decrypts an encrypted password using a provided private key in PEM format.
        /// </summary>
        /// <param name="encryptedPassword">The encrypted password to be decrypted (in Base64 format).</param>
        /// <param name="privateKey">The private key in PEM format used for decryption.</param>
        /// <returns>The decrypted password as a string.</returns>
        public string DecryptPassword(string encryptedPassword, string privateKey)
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(privateKey.ToCharArray());

            var encryptedBytes = Convert.FromBase64String(encryptedPassword);
            var decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.Pkcs1);

            return Encoding.UTF8.GetString(decryptedBytes);
        }

        /// <summary>
        /// Encrypts a plaintext string using AES encryption.
        /// </summary>
        /// <param name="plainText">The plaintext string to be encrypted.</param>
        /// <param name="key">Optional AES key. If not provided, the key from configuration is used.</param>
        /// <returns>The encrypted ciphertext as a Base64 encoded string.</returns>
        public async Task<string> AesEncryptionAsync(string plainText, byte[]? key = null)
        {
            var keyBytes = key ?? GetKeyFromConfig();
            var ivBytes = GetIvFromConfig();

            using var aes = Aes.Create();
            aes.Key = keyBytes;
            aes.IV = ivBytes;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                using var sw = new StreamWriter(cs);
                await sw.WriteAsync(plainText);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// Decrypts an AES-encrypted ciphertext string.
        /// </summary>
        /// <param name="cipherText">The Base64 encoded ciphertext string to be decrypted.</param>
        /// <param name="key">Optional AES key. If not provided, the key from configuration is used.</param>
        /// <returns>The decrypted plaintext string.</returns>
        public async Task<string> AesDecryptionAsync(string cipherText, byte[]? key = null)
        {
            var keyBytes = key ?? GetKeyFromConfig();
            var ivBytes = GetIvFromConfig();

            using var aes = Aes.Create();
            aes.Key = keyBytes;
            aes.IV = ivBytes;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return await sr.ReadToEndAsync();
        }
    }
}