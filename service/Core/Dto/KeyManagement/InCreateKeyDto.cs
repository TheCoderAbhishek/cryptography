namespace service.Core.Dto.KeyManagement
{
    /// <summary>
    /// Represent a create key DTO.
    /// </summary>
    public class InCreateKeyDto
    {
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
        /// Gets or sets the intended usage of the key.
        /// </summary>
        public string? KeyUsage { get; set; }
    }
}
