namespace service.Infrastructure.Queries.Account
{
    /// <summary>
    /// Provides a set of queries related to account management.
    /// </summary>
    public static class AccountQueries
    {
        /// <summary>
        /// SQL query to retrieves user information based on the provided email address.
        /// </summary>
        public const string GetUserEmail = "SELECT * FROM [cryptography].[dbo].[User] WHERE Email = @Email";

        /// <summary>
        /// SQL query to retrieve user record based upon Username or Email;
        /// </summary>
        public const string GetUserUsernameEmail = "SELECT Id FROM [cryptography].[dbo].[User] WHERE Email = @Email OR UserName = @UserName";

        /// <summary>
        /// SQL query to add new user into User table.
        /// </summary>
        public const string AddNewUser = "INSERT INTO [cryptography].[dbo].[User] (UserId, Name, UserName, Email, Password, IsAdmin, IsActive, IsLocked, IsDeleted, LoginAttempts, DeletedStatus, CreatedOn, UpdatedOn, DeletedOn, AutoDeletedOn, LastLoginDateTime, LockedUntil, RoleId, Salt) VALUES (@UserId, @Name, @UserName, @Email, @Password, @IsAdmin, @IsActive, @IsLocked, @IsDeleted, @LoginAttempts, @DeletedStatus, @CreatedOn, @UpdatedOn, @DeletedOn, @AutoDeletedOn, @LastLoginDateTime, @LockedUntil, @RoleId, @Salt); SELECT CAST(SCOPE_IDENTITY() as int);";

        /// <summary>
        /// SQL query string to retrieve all users from the User table.
        /// </summary>
        public const string GetAllUsers = "SELECT * FROM [cryptography].[dbo].[User]";

        /// <summary>
        /// SQL query to retrieve ID by email from user table.
        /// </summary>
        public const string GetIdEmail = "SELECT Id FROM [cryptography].[dbo].[User] WHERE Email = @Email";

        /// <summary>
        /// SQL query to retrieve ID and OTP Valid TIme by email from user table.
        /// </summary>
        public const string GetIdValidUntilEmail = "SELECT * FROM [cryptography].[dbo].[OtpStorage] WHERE Email = @Email";

        /// <summary>
        /// Update OTP if it's invalid
        /// </summary>
        public const string UpdateOtpInValid = "UPDATE [cryptography].[dbo].[OtpStorage] SET GeneratedOn = @GeneratedOn, ValidUntil = @ValidUntil, Otp = @Otp WHERE Email = @Email";
    }
}
