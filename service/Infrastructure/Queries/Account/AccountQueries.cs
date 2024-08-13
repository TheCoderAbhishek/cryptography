namespace service.Infrastructure.Queries.Account
{
    /// <summary>
    /// Provides a set of queries related to account management.
    /// </summary>
    public static class AccountQueries
    {
        /// <summary>
        /// SQL query to add new user into User table.
        /// </summary>
        public const string AddNewUser = "INSERT INTO [cryptography].[dbo].[User] (UserId, Name, UserName, Email, Password, IsAdmin, IsActive, IsLocked, IsDeleted, LoginAttempts, DeletedStatus, CreatedOn, UpdatedOn, DeletedOn, AutoDeletedOn, LastLoginDateTime, LockedUntil, RoleId, Salt) VALUES (@UserId, @Name, @UserName, @Email, @Password, @IsAdmin, @IsActive, @IsLocked, @IsDeleted, @LoginAttempts, @DeletedStatus, @CreatedOn, @UpdatedOn, @DeletedOn, @AutoDeletedOn, @LastLoginDateTime, @LockedUntil, @RoleId, @Salt); SELECT CAST(SCOPE_IDENTITY() as int);";

        /// <summary>
        /// SQL query string to retrieve all users from the User table.
        /// </summary>
        public const string GetAllUsers = "SELECT * FROM [cryptography].[dbo].[User]";
    }
}
