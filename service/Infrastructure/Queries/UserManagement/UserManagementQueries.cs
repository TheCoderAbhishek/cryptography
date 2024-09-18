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
    }
}
