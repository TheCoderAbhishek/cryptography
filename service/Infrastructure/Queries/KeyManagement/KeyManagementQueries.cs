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

        /// <summary>
        /// Checks if a given KeyName exists uniquely in the tblKeys table.
        /// Returns 1 if the KeyName is unique, 0 otherwise.
        /// </summary>
        public const string _checkKeyNameUniqueOrNot = @"SELECT CASE WHEN COUNT(*) > 0 THEN 0 ELSE 1 END AS IsUnique" +
                                                            " FROM [cryptography].[dbo].[tblKeys]" +
                                                            " WHERE KeyName = @KeyName;";

        /// <summary>
        /// Inserts a new record into the `tblSecureKeys` table.
        /// </summary>
        public const string _insertPrivateDataTable = @"INSERT INTO [cryptography].[dbo].[tblSecureKeys] (KeyId, KeyName, KeyType, KeyAlgorithm, KeySize, KeyOwner, KeyStatus, KeyAccess, KeyMaterial)" +
                                                            " VALUES (@KeyId, @KeyName, @KeyType, @KeyAlgorithm, @KeySize, @KeyOwner, @KeyStatus, @KeyAccess, @KeyMaterial);";

        /// <summary>
        /// Checks if a given KeyId exists uniquely in the tblKeys table.
        /// Returns 1 if the KeyId is unique, 0 otherwise.
        /// </summary>
        public const string _checkKeyIdUniqueOrNot = @"SELECT CASE WHEN COUNT(*) > 0 THEN 0 ELSE 1 END AS IsUnique" +
                                                            " FROM [cryptography].[dbo].[tblKeys]" +
                                                            " WHERE KeyId = @KeyId;";

        /// <summary>
        /// SQL query to get key material from keys table.
        /// </summary>
        public const string _getKeyDataAssociatedWithId = @"SELECT KeyMaterial FROM [cryptography].[dbo].[tblKeys] WHERE id = @Id;";

        /// <summary>
        /// SQL query to get key details associated with id.
        /// </summary>
        public const string _getKeyDetailsById = @"SELECT * FROM [cryptography].[dbo].[tblKeys] WHERE id = @Id;";
    }
}
