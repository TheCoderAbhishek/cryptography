namespace service.Core.Entities.AccountManagement
{
    /// <summary>
    /// Represents a user in the system.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        public string? UserId { get; set; }
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
        /// Indicates whether the user is an administrator.
        /// </summary>
        public bool? IsAdmin { get; set; }

        /// <summary>
        /// Indicates whether the user account is active.
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Indicates whether the user account is locked.
        /// </summary>
        public bool? IsLocked { get; set; }

        /// <summary>
        /// Indicates whether the user account is deleted.
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the number of failed login attempts.
        /// </summary>
        public int LoginAttempts { get; set; }

        /// <summary>
        /// Gets or sets the deleted status of the user.
        /// </summary>
        public DeletedState? DeletedStatus { get; set; }

        /// <summary>
        /// Gets the date and time when the user account was created.
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the user account was last updated.
        /// </summary>
        public DateTime? UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the user account was deleted.
        /// </summary>
        public DateTime? DeletedOn { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the user account is scheduled for automatic deletion.
        /// </summary>
        public DateTime? AutoDeletedOn { get; set; }

        /// <summary>
        /// Gets or sets the date and time of the user's last login.
        /// </summary>
        public DateTime? LastLoginDateTime { get; set; }

        /// <summary>
        /// Gets or sets the date and time until the user account is locked.
        /// </summary>
        public DateTime? LockedUntil { get; set; }

        /// <summary>
        /// Gets or sets the role ID of the user.
        /// </summary>
        public RoleId? RoleId { get; set; }

        /// <summary>
        /// Gets or sets the salt used for password hashing.
        /// </summary>
        public string? Salt { get; set; }
    }

    /// <summary>
    /// Enumeration representing the different deleted states of a User record.
    /// </summary>
    public enum RoleId
    {
        /// <summary>
        /// User have only view access.
        /// </summary>
        Viewer = 0,

        /// <summary>
        /// User have only using access.
        /// </summary>
        User = 1,

        /// <summary>
        /// User have limited editing access.
        /// </summary>
        Admin = 2,

        /// <summary>
        /// User have full access.
        /// </summary>
        SuperAdmin = 3,
    }

    /// <summary>
    /// Enumeration representing the different deleted states of a User record.
    /// </summary>
    public enum DeletedState
    {
        /// <summary>
        /// User record is not deleted.
        /// </summary>
        NotDeleted = 0,

        /// <summary>
        /// User record is in deleted state.
        /// </summary>
        SoftDeleted = 1,

        /// <summary>
        /// User record is deleted.
        /// </summary>
        HardDeleted = 2
    }
}
