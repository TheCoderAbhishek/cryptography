namespace service.Core.Commands
{
    /// <summary>
    /// Provides a collection of Open SSL commands used within the service.
    /// </summary>
    public static class OpenSslCommands
    {
        /// <summary>
        /// Generates a random AES key data string with a length of 32 bytes.
        /// </summary>
        public const string GenerateAes256KeyData = @"openssl rand -base64 32";

        /// <summary>
        /// Generates a random AES key data string with a length of 24 bytes.
        /// </summary>
        public const string GenerateAes192KeyData = @"openssl rand -base64 24";

        /// <summary>
        /// Generates a random AES key data string with a length of 16 bytes.
        /// </summary>
        public const string GenerateAes128KeyData = @"openssl rand -base64 16";
    }
}