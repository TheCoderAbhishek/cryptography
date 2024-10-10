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

        /// <summary>
        /// Generates an RSA private key with a length of 2048 bits.
        /// </summary>
        public const string GenerateRsa2048PrivateKey = @"openssl genpkey -algorithm RSA -pkeyopt rsa_keygen_bits:2048";

        /// <summary>
        /// Generates an RSA private key with a length of 3072 bits.
        /// </summary>
        public const string GenerateRsa3072PrivateKey = @"openssl genpkey -algorithm RSA -pkeyopt rsa_keygen_bits:3072";

        /// <summary>
        /// Generates an RSA private key with a length of 4096 bits.
        /// </summary>
        public const string GenerateRsa4096PrivateKey = @"openssl genpkey -algorithm RSA -pkeyopt rsa_keygen_bits:4096";

        /// <summary>
        /// Extracts the public key from an existing RSA private key.
        /// </summary>
        public const string ExtractPublicKeyFromPrivateKey = @"openssl rsa -pubout";
    }
}