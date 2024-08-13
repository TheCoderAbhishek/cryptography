namespace service.Infrastructure.Queries.Account
{
    /// <summary>
    /// Provides a set of queries related to account management.
    /// </summary>
    public static class AccountQueries
    {
        /// <summary>
        /// SQL query string to retrieve all users from the Employees table.
        /// </summary>
        public const string GetAllUsers = "SELECT * FROM Employees";
    }
}
