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

        /// <summary>
        /// SQL query to insert key details in table.
        /// </summary>
        public const string _createKey = @"INSERT INTO [cryptography].[dbo].[tblKeys]" +
                                            "(KeyId, KeyName, KeyType, KeyAlgorithm, KeySize, KeyOwner, KeyStatus, KeyState, KeyAccess, KeyUsage, KeyCreatedOn, KeyUpdatedOn, KeyMaterial)" +
                                            "VALUES(@KeyId, @KeyName, @KeyType, @KeyAlgorithm, @KeySize, @KeyOwner, @KeyStatus, @KeyState, @KeyAccess, @KeyUsage, @KeyCreatedOn, @KeyUpdatedOn, @KeyMaterial)";
    }
}
