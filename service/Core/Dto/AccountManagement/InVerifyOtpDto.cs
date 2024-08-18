namespace service.Core.Dto.AccountManagement
{
    /// <summary>
    /// Data transfer object (DTO) for verifying an OTP (One-Time Password).
    /// </summary>
    public class InVerifyOtpDto
    {
        /// <summary>
        /// Gets or sets the email address associated with the OTP.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the OTP (One-Time Password) to be verified.
        /// </summary>
        public string? Otp { get; set; }
    }
}
