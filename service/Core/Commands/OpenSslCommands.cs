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
        public const string _generateAesKeyData = @"openssl rand -hex 32";
    }
}
