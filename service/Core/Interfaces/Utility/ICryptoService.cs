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
    }
}
