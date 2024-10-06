namespace service.Core.Dto.UserManagement
{
    /// <summary>
    /// Represents the input data for updating user details.
    /// </summary>
    public class InUpdateUserDetails
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user to be updated.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the updated name of the user.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the updated username of the user.
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Gets or sets the updated email address of the user.
        /// </summary>
        public string? Email { get; set; }
    }
}