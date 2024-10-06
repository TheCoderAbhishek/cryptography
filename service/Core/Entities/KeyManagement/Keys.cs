namespace service.Core.Entities.KeyManagement
{
    /// <summary>
    /// Represents a key.
    /// </summary>
    public class Keys
    {
        /// <summary>
        /// Gets or sets the unique identifier for the key.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the key.
        /// </summary>
        public string? KeyId { get; set; }

        /// <summary>
        /// Gets or sets the name of the key.
        /// </summary>
        public string? KeyName { get; set; }

        /// <summary>
        /// Gets or sets the type of the key.
        /// </summary>
        public string? KeyType { get; set; }

        /// <summary>
        /// Gets or sets the algorithm used to generate the key.
        /// </summary>
        public string? KeyAlgorithm { get; set; }

        /// <summary>
        /// Gets or sets the size of the key in bits.
        /// </summary>
        public int KeySize { get; set; }

        /// <summary>
        /// Gets or sets the owner of the key.
        /// </summary>
        public string? KeyOwner { get; set; }

        /// <summary>
        /// Gets or sets the status of the key (active or inactive).
        /// </summary>
        public bool? KeyStatus { get; set; }

        /// <summary>
        /// Gets or sets the state of the key (e.g., pending, active, revoked).
        /// </summary>
        public int KeyState { get; set; }

        /// <summary>
        /// Gets or sets the access permissions for the key.
        /// </summary>
        public string? KeyAccess { get; set; }

        /// <summary>
        /// Gets or sets the intended usage of the key.
        /// </summary>
        public string? KeyUsage { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the key was created.
        /// </summary>
        public DateTime? KeyCreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the key was last updated.
        /// </summary>
        public DateTime? KeyUpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets the actual key material (encrypted or raw).
        /// </summary>
        public string? KeyMaterial { get; set; }
    }
}
