namespace service.Core.Dto.UserManagement
{
    /// <summary>
    /// Represent a DTO for creating new user.
    /// </summary>
    public class InCreateUser
    {
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the user's password.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets the user's role.
        /// </summary>
        public int RoleId { get; set; }
    }
}
