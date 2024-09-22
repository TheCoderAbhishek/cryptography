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
        public const string _getAllUsers = @"SELECT * FROM [cryptography].[dbo].[tblUsers] WHERE IsDeleted=0";

        /// <summary>
        /// SQL query string to retrieve deleted users from the tblUsers table.
        /// </summary>
        public const string _getSoftDeletedUsers = @"SELECT * FROM [cryptography].[dbo].[tblUsers] WHERE IsDeleted=1";

        /// <summary>
        /// SQL query string to get user details based upon ID.
        /// </summary>
        public const string _getUserDetailsUponId = @"SELECT * FROM [cryptography].[dbo].[tblUsers] WHERE Id=@Id";

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

        /// <summary>
        /// SQL query string to soft delete user based upon Id
        /// </summary>
        public const string _softDeleteUser = @"UPDATE [cryptography].[dbo].[tblUsers]" +
                                                " SET IsDeleted = @IsDeleted, DeletedStatus = @DeletedStatus, UpdatedOn = @UpdatedOn, DeletedOn = @DeletedOn, AutoDeletedOn = @AutoDeletedOn" +
                                                " WHERE Id = @Id;";
    }
}
