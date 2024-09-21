namespace service.Infrastructure.Queries.UserManagement
{
    /// <summary>
    /// Contains SQL queries for user management operations.
    /// </summary>
    public static class UserManagementQueries
    {
        /// <summary>
        /// SQL query string to retrieve all users from the User table.
        /// </summary>
        public const string GetAllUsers = @"SELECT * FROM [cryptography].[dbo].[tblUsers]";

        /// <summary>
        /// SQL query string to insert new user details in table.
        /// </summary>
        public const string _createNewUser = @"INSERT INTO [cryptography].[dbo].[tblUsers] (UserId, Name, UserName, Email, Password, IsAdmin, IsActive, IsLocked, IsDeleted, LoginAttempts, DeletedStatus, CreatedOn, RoleId, Salt)" +
                                                " VALUES (@UserId, @Name, @UserName, @Email, @Password, @IsAdmin, @IsActive, @IsLocked, @IsDeleted, @LoginAttempts, @DeletedStatus, @CreatedOn, @RoleId, @Salt);" +
                                                " SELECT CAST(SCOPE_IDENTITY() as int);";

        /// <summary>
        /// SQL query string to fetch user details based upon email or username
        /// </summary>
        public const string _getUserDetailsMailUsernameAsync = @"SELECT CASE WHEN EXISTS (SELECT 1 FROM [cryptography].[dbo].[tblUsers] WHERE Email = @Email) THEN 1" +
                                                                    " WHEN EXISTS (SELECT 1 FROM [cryptography].[dbo].[tblUsers] WHERE Username = @Username) THEN 2" +
                                                                    " ELSE 0" +
                                                                    " END AS DuplicateStatus;";

        /// <summary>
        /// SQL query string to update user locked/unlocked status.
        /// </summary>
        public const string _lockUnlockUser = @"UPDATE [cryptography].[dbo].[tblUsers]" +
                                                " SET IsLocked = CASE WHEN IsLocked = 1 THEN 0 ELSE 1 END, UpdatedOn = @UpdatedOn" +
                                                " WHERE id = @Id;";
    }
}
