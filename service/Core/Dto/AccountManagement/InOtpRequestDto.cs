namespace service.Core.Dto.AccountManagement
{
    /// <summary>
    /// This class represents the InOtpRequestDto data transfer object.
    /// It contains the email address for which an OTP is requested.
    /// </summary>
    public class InOtpRequestDto
    {
        /// <summary>
        /// Gets or sets the email address for which an OTP is requested.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the purpose for which the OTP is requested.
        /// </summary>
        public OtpUse? Use { get; set; }
    }

    /// <summary>
    /// This enumeration defines the possible purposes for requesting an OTP.
    /// </summary>
    public enum OtpUse
    {
        /// <summary>
        /// The OTP is used for account activation.
        /// </summary>
        AccountActivate = 1,

        /// <summary>
        /// The OTP is used for forgotten client password reset.
        /// </summary>
        ForgotClientPassword = 2
    }
}
