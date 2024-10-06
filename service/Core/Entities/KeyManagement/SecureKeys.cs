namespace service.Core.Entities.KeyManagement
{
    /// <summary>
    /// Represents a secure key.
    /// </summary>
    public class SecureKeys
    {
        /// <summary>
        /// Gets or sets the unique identifier for the secure key.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the secure key.
        /// </summary>
        public string? KeyId { get; set; }

        /// <summary>
        /// Gets or sets the name of the secure key.
        /// </summary>
        public string? KeyName { get; set; }

        /// <summary>
        /// Gets or sets the type of the secure key.
        /// </summary>
        public string? KeyType { get; set; }

        /// <summary>
        /// Gets or sets the algorithm used to generate the secure key.
        /// </summary>
        public string? KeyAlgorithm { get; set; }

        /// <summary>
        /// Gets or sets the size of the secure key in bits.
        /// </summary>
        public int KeySize { get; set; }

        /// <summary>
        /// Gets or sets the owner of the secure key.
        /// </summary>
        public string? KeyOwner { get; set; }

        /// <summary>
        /// Gets or sets the status of the secure key (active or inactive).
        /// </summary>
        public bool? KeyStatus { get; set; }

        /// <summary>
        /// Gets or sets the access permissions for the secure key.
        /// </summary>
        public string? KeyAccess { get; set; }

        /// <summary>
        /// Gets or sets the actual key material (encrypted or raw).
        /// </summary>
        public string? KeyMaterial { get; set; }
    }
}
