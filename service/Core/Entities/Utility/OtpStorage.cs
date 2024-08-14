namespace service.Core.Entities.Utility
{
    /// <summary>
    /// Represent OTP storage in the system.
    /// </summary>
    public class OtpStorage
    {
        /// <summary>
        /// Gets or sets the unique identifier for the OTP record.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the user Id associated with the email.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the email address associated with the OTP.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the OTP was generated.
        /// </summary>
        public DateTime GeneratedOn { get; set; }

        /// <summary>
        /// Gets or sets the date and time until which the OTP is valid.
        /// </summary>
        public DateTime ValidUntil { get; set; }

        /// <summary>
        /// Gets or sets the OTP value.
        /// </summary>
        public string? Otp { get; set; }

        /// <summary>
        /// Gets or sets the salt used for securing the OTP.
        /// </summary>
        public string? Salt { get; set; }

        /// <summary>
        /// Gets or sets the number of attempts made to use the OTP.
        /// </summary>
        public int AttemptCount { get; set; }

        /// <summary>
        /// Gets or sets the use of OTP.
        /// </summary>
        public int OtpUseCase { get; set; }
    }
}
