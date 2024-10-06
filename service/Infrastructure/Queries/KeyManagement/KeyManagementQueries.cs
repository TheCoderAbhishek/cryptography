namespace service.Infrastructure.Queries.KeyManagement
{
    /// <summary>
    /// Contains SQL queries related to key management.
    /// </summary>
    public static class KeyManagementQueries
    {
        /// <summary>
        /// SQL query to retrieve a list of all keys.
        /// </summary>
        public const string _getKeyList = @"SELECT * FROM [cryptography].[dbo].[tblKeys]";
    }
}
