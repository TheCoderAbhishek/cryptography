﻿using Microsoft.Data.SqlClient;
using service.Core.Entities.KeyManagement;
using service.Core.Enums;
using service.Core.Interfaces.KeyManagement;
using service.Core.Interfaces.Utility;
using service.Infrastructure.Dependency;
using service.Infrastructure.Queries.KeyManagement;

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
    }
}