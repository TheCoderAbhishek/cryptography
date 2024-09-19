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
        public const string GetAllUsers = "SELECT * FROM [cryptography].[dbo].[tblUsers]";

        /// <summary>
        /// SQL query string to insert new user details in table.
        /// </summary>
        public const string _createNewUser = "INSERT INTO [cryptography].[dbo].[tblUsers] (UserId, Name, UserName, Email, Password, IsAdmin, IsActive, IsLocked, IsDeleted, LoginAttempts, DeletedStatus, CreatedOn, RoleId, Salt)" + 
                                                "VALUES (@UserId, @Name, @UserName, @Email, @Password, @IsAdmin, @IsActive, @IsLocked, @IsDeleted, @LoginAttempts, @DeletedStatus, @CreatedOn, @RoleId, @Salt);" +
                                                "SELECT CAST(SCOPE_IDENTITY() as int);";
    }
}
