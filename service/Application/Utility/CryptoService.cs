using service.Core.Interfaces.Utility;
using System.Security.Cryptography;
using System.Text;

namespace service.Application.Utility
{
    /// <summary>
    /// Provides an implementation of the ICryptoService interface for cryptographic operations, 
    /// focusing on password decryption using RSA.
    /// </summary>
    public class CryptoService : ICryptoService
    {
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
    }
}