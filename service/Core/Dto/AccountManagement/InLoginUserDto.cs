namespace service.Core.Dto.AccountManagement
{
    /// <summary>
    /// This class represents the InLoginUserDto object used for login requests.
    /// </summary>
    public class InLoginUserDto
    {
        /// <summary>
        /// Gets or sets the email address of the client attempting to login.
        /// </summary>
        public string? UserEmail { get; set; }

        /// <summary>
        /// Gets or sets the password of the client attempting to login.
        /// </summary>
        public string? UserPassword { get; set; }
    }
}
