using service.Core.Entities.AccountManagement;

namespace service.Core.Dto.AccountManagement
{
    /// <summary>
    /// This class represents the LoginResponseDto object used in account management.
    /// </summary>
    public class LoginResponseDto
    {
        /// <summary>
        /// Gets or sets the access token obtained after a successful login.
        /// </summary>
        public string? Token { get; set; }
        /// <summary>
        /// Gets or sets the client information associated with the login.
        /// </summary>
        public User? User { get; set; }
        /// <summary>
        /// Gets or sets a collection of claims associated with the logged-in user.
        /// </summary>
        public Dictionary<string, string>? Claims { get; set; }
    }
}
