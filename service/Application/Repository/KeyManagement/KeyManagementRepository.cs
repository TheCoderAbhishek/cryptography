using Microsoft.Data.SqlClient;
using service.Core.Entities.KeyManagement;
using service.Core.Entities.Utility;
using service.Core.Enums;
using service.Core.Interfaces.KeyManagement;
using service.Core.Interfaces.Utility;
using service.Infrastructure.Dependency;
using service.Infrastructure.Queries.KeyManagement;
using System.Security.Cryptography;

namespace service.Application.Repository.KeyManagement
{
    /// <summary>
    /// Provides repository functionality for managing keys in the system.
    /// Implements the <see cref="IKeyManagementRepository"/> interface.
    /// </summary>
    public class KeyManagementRepository(ILogger<KeyManagementRepository> logger, ICommonDbHander commonDbHander) : IKeyManagementRepository
    {
        private readonly ILogger<KeyManagementRepository> _logger = logger;
        private readonly ICommonDbHander _commonDbHander = commonDbHander;

        /// <summary>
        /// Asynchronously retrieves a list of keys from the database.
        /// </summary>
        /// <returns>A task that returns a list of <see cref="Keys"/> objects, or an empty list if no keys are found.</returns>
        public async Task<List<Keys>> GetKeysListAsync()
        {
            string query = KeyManagementQueries._getKeyList;

            try
            {
                _logger.LogInformation("Attempting to fetch key list from database using query: {Query}", query);

                // Fetch data from the database
                List<Keys> keysList = await _commonDbHander.GetData<Keys>(query,
                    "Fetched all keys successfully",
                    "Error fetching keys",
                    ErrorCode.GetKeysListAsyncError,
                    ConstantData.Txn());

                if (keysList == null || keysList.Count == 0)
                {
                    _logger.LogWarning("No keys found in the database.");
                    return [];
                }

                _logger.LogInformation("{Count} keys found and fetched successfully.", keysList.Count);
                return keysList;
            }
            catch (SqlException sqlException)
            {
                _logger.LogError(sqlException, "A SQL error occurred while fetching the key list. Error: {ErrorMessage}", sqlException.Message);
                throw new CustomException("An SQL error occurred while fetching keys from the key management repository.", sqlException,
                    ErrorCode.GetKeysListAsyncSqlException, ConstantData.Txn());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching keys. {ErrorMessage}", ex.Message);
                throw new CustomException("An unexpected error occurred while fetching keys from the key management repository.", ex,
                    ErrorCode.GetKeysListAsyncException, ConstantData.Txn());
            }
        }

        /// <summary>
        /// Creates a new key in the key management repository.
        /// </summary>
        /// <param name="key">The key object to be created.</param>
        /// <returns>The status of the operation (1 for success, 0 for failure).</returns>
        /// <exception cref="CustomException">Thrown if an error occurs during the operation.</exception>
        public async Task<int> CreateKeyAsync(Keys key)
        {
            string query = KeyManagementQueries._createKey;

            try
            {
                _logger.LogInformation("Attempting to insert key details using query: {Query}", query);

                var parameters = new
                {
                    key.KeyId,
                    key.KeyName,
                    key.KeyType,
                    key.KeyAlgorithm,
                    key.KeySize,
                    key.KeyOwner,
                    key.KeyStatus,
                    key.KeyState,
                    key.KeyAccess,
                    key.KeyUsage,
                    key.KeyCreatedOn,
                    key.KeyUpdatedOn,
                    key.KeyMaterial
                };

                BaseResponse baseResponse = await _commonDbHander.AddUpdateDeleteData(query,
                    "Key created successfully.",
                    "An error occurred while creating the key.",
                    "Duplicate key record found.",
                    ErrorCode.CreateKeyAsyncError,
                    ConstantData.Txn(),
                    parameters
                    );

                return baseResponse.Status;
            }
            catch (SqlException sqlException)
            {
                _logger.LogError(sqlException, "A SQL error occurred while inserting key details. Error: {ErrorMessage}", sqlException.Message);
                throw new CustomException("An SQL error occurred while inserting key details from the key management repository.", sqlException,
                    ErrorCode.CreateKeyAsyncSqlException, ConstantData.Txn());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while inserting key details. {ErrorMessage}", ex.Message);
                throw new CustomException("An unexpected error occurred while inserting key details from the key management repository.", ex,
                    ErrorCode.CreateKeyAsyncException, ConstantData.Txn());
            }
        }

        /// <summary>
        /// Checks if the specified key name is unique in the database.
        /// </summary>
        /// <param name="keyName">The key name to check.</param>
        /// <returns>
        /// 1 if the key name is unique, 0 if it is not unique.
        /// </returns>
        /// <exception cref="CustomException">Thrown if an error occurs during the check.</exception>
        public async Task<int> CheckUniqueKeyName(string keyName)
        {
            string query = KeyManagementQueries._checkKeyNameUniqueOrNot;
            try
            {
                _logger.LogInformation("Starting unique key name check for key: {KeyName}", keyName);

                var parameters = new
                {
                    @KeyName = keyName,
                };

                int isUnique = await _commonDbHander.GetSingleData<int>(query,
                    $"Unique key name check successful for key: {keyName}",
                    $"Error checking unique key name for key: {keyName}",
                    ErrorCode.CheckUniqueKeyNameError,
                    ConstantData.Txn(),
                    parameters);

                if (isUnique == 1)
                {
                    _logger.LogInformation("Unique key name check successful for key: {KeyName}", keyName);
                }
                else
                {
                    _logger.LogError("Error checking unique key name for key: {KeyName}", keyName);
                }

                return isUnique;
            }
            catch (SqlException sqlException)
            {
                _logger.LogError(sqlException, "SQL exception occurred during unique key name check: {ErrorMessage}", sqlException.Message);
                throw new CustomException("SQL exception occurred during unique key name check", sqlException,
                    ErrorCode.CheckUniqueKeyNameSqlException, ConstantData.Txn());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during unique key name check: {ErrorMessage}", ex.Message);
                throw new CustomException("Exception occurred during unique key name check", ex,
                    ErrorCode.CheckUniqueKeyNameException, ConstantData.Txn());
            }
        }

        /// <summary>
        /// Inserts private data into the database.
        /// </summary>
        /// <param name="secureKeys">The secure keys to insert.</param>
        /// <returns>The status of the insertion operation.</returns>
        /// <exception cref="CustomException">Thrown if an error occurs during the insertion.</exception>
        public async Task<int> InsertPrivateDataAsync(SecureKeys secureKeys)
        {
            string query = KeyManagementQueries._insertPrivateDataTable;
            try
            {
                _logger.LogInformation("");

                var parameters = new
                {
                    secureKeys.KeyId,
                    secureKeys.KeyName,
                    secureKeys.KeyType,
                    secureKeys.KeyAlgorithm,
                    secureKeys.KeySize,
                    secureKeys.KeyOwner,
                    secureKeys.KeyStatus,
                    secureKeys.KeyAccess,
                    secureKeys.KeyMaterial
                };

                BaseResponse baseResponse = await _commonDbHander.AddUpdateDeleteData(query,
                    $"Private data inserted successfully for KeyId: {secureKeys.KeyId}",
                    $"Failed to insert private data for KeyId: {secureKeys.KeyId}.",
                    $"Failed to insert private data for KeyId: {secureKeys.KeyId}. Duplicate record found.",
                    ErrorCode.InsertPrivateDataAsyncError,
                    ConstantData.Txn(),
                    parameters);

                return baseResponse.Status;
            }
            catch (SqlException sqlException)
            {
                _logger.LogError(sqlException, "An error occurred while inserting private data into the database: {ErrorMessage}", sqlException.Message);
                throw new CustomException("An error occurred while inserting private data into the database.", sqlException,
                    ErrorCode.InsertPrivateDataAsyncSqlException, ConstantData.Txn());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while inserting private data: {ErrorMessage}", ex.Message);
                throw new CustomException("An unexpected error occurred while inserting private data.", ex,
                    ErrorCode.InsertPrivateDataAsyncException, ConstantData.Txn());
            }
        }
    }
}
