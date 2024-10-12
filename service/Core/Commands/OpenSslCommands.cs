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

        /// <summary>
        /// Generates a private key using the ECC algorithm with the P-160 curve.
        /// This command utilizes OpenSSL to create a private key suitable for 
        /// cryptographic operations that require a 160-bit key length.
        /// </summary>
        public const string GenerateEcc160PrivateKey = @"openssl genpkey -algorithm EC -pkeyopt ec_paramgen_curve:P-160";

        /// <summary>
        /// Generates a private key using the ECC algorithm with the P-224 curve.
        /// This command utilizes OpenSSL to create a private key suitable for 
        /// cryptographic operations that require a 224-bit key length.
        /// </summary>
        public const string GenerateEcc224PrivateKey = @"openssl genpkey -algorithm EC -pkeyopt ec_paramgen_curve:P-224";

        /// <summary>
        /// Generates a private key using the ECC algorithm with the P-256 curve.
        /// This command utilizes OpenSSL to create a private key suitable for 
        /// cryptographic operations that require a 256-bit key length.
        /// </summary>
        public const string GenerateEcc256PrivateKey = @"openssl genpkey -algorithm EC -pkeyopt ec_paramgen_curve:P-256";

        /// <summary>
        /// Generates a private key using the ECC algorithm with the P-384 curve.
        /// This command utilizes OpenSSL to create a private key suitable for 
        /// cryptographic operations that require a 384-bit key length.
        /// </summary>
        public const string GenerateEcc384PrivateKey = @"openssl genpkey -algorithm EC -pkeyopt ec_paramgen_curve:P-384";

        /// <summary>
        /// Generates a private key using the ECC algorithm with the P-521 curve.
        /// This command utilizes OpenSSL to create a private key suitable for 
        /// cryptographic operations that require a 521-bit key length.
        /// </summary>
        public const string GenerateEcc521PrivateKey = @"openssl genpkey -algorithm EC -pkeyopt ec_paramgen_curve:P-521";

        /// <summary>
        /// Command to extract the public key from a given ECC private key using OpenSSL.
        /// This command outputs the public key in PEM format.
        /// </summary>
        public const string ExtractPublicKeyFromPrivateKeyEcc = @"openssl ec -pubout";

        /// <summary>
        /// Command to generate a 112-bit DES key directly in hexadecimal format using OpenSSL.
        /// This command produces a random key of 14 bytes, suitable for DES encryption.
        /// </summary>
        public const string GenerateDes112KeyData = @"openssl rand -hex 14";

        /// <summary>
        /// Command to generate a 168-bit DES key directly in hexadecimal format using OpenSSL.
        /// This command produces a random key of 21 bytes, suitable for DES encryption.
        /// </summary>
        public const string GenerateDes168KeyData = @"openssl rand -hex 21";
    }
}