namespace service.Infrastructure.Dependency
{
    /// <summary>
    /// Holds configuration settings for a SMTP server.
    /// </summary>
    public class SmtpSettings
    {
        /// <summary>
        /// Gets or sets the hostname or IP address of the SMTP server.
        /// </summary>
        public string? Host { get; set; }
        /// <summary>
        /// Gets or sets the port number used by the SMTP server.
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Gets or sets the username for authentication with the SMTP server.
        /// </summary>
        public string? Username { get; set; }
        /// <summary>
        /// Gets or sets the password for authentication with the SMTP server.
        /// </summary>
        public string? Password { get; set; }
    }
}
