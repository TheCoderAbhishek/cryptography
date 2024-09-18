﻿using Microsoft.Data.SqlClient;
using service.Core.Entities.AccountManagement;
using service.Core.Entities.Utility;
using service.Core.Enums;
using service.Core.Interfaces.UserManagement;
using service.Core.Interfaces.Utility;
using service.Infrastructure.Dependency;
using service.Infrastructure.Queries.UserManagement;

namespace service.Application.Repository.UserManagement
{
    /// <summary>
    /// Implements the <see cref="IUserManagementRepository"/> interface for user management operations.
    /// </summary>
    public class UserManagementRepository(ILogger<UserManagementRepository> logger, ICommonDbHander commonDbHander) : IUserManagementRepository
    {
        private readonly ILogger<UserManagementRepository> _logger = logger;
        private readonly ICommonDbHander _commonDbHander = commonDbHander;

        /// <inheritdoc />
        public async Task<List<User>> GetUsersAsync()
        {
            string query = UserManagementQueries.GetAllUsers;

            try
            {
                List<User> users = await _commonDbHander.GetData<User>(query, "Fetched all users successfully",
                                                            "Error fetching users", ErrorCode.GetUsersAsyncError, ConstantData.Txn());

                if (users == null || users.Count == 0)
                {
                    _logger.LogWarning("No users found in the database.");
                }
                return users!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching users. {Message}", ex.Message);
                throw new CustomException("Error fetching users from the account repository.", ex,
                                   ErrorCode.GetUsersAsyncException, ConstantData.Txn());
            }
        }

        /// <inheritdoc />
        public async Task<int> CreateNewUserAsync(User user)
        {
            string query = UserManagementQueries._createNewUser;

            try
            {
                var parameters = new
                {
                    user.UserId,
                    user.Name,
                    user.UserName,
                    user.Email,
                    user.Password,
                    user.IsAdmin,
                    user.IsActive,
                    user.IsLocked,
                    user.IsDeleted,
                    user.LoginAttempts,
                    user.DeletedStatus,
                    user.CreatedOn,
                    user.UpdatedOn,
                    user.DeletedOn,
                    user.AutoDeletedOn,
                    user.LastLoginDateTime,
                    user.LockedUntil,
                    user.RoleId,
                    user.Salt
                };

                BaseResponse baseResponse = await _commonDbHander.AddDataReturnLatestId(query,
                    "New user added successfully.",
                    "An error occurred while adding the new user. Please try again.",
                    "A user with the same username or email already exists.",
                    ErrorCode.CreateNewUserAsyncError,
                    ConstantData.Txn(), parameters);

                return baseResponse.Status;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL error occurred while inserting user with UserId {UserId}. {Message}", user.UserId, sqlEx.Message);
                throw new CustomException("SQL error while adding user.", sqlEx, ErrorCode.CreateNewUserAsyncSqlException, ConstantData.Txn());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while inserting user in table. {Message}", ex.Message);
                throw new CustomException("Error inserting user from the account repository.", ex,
                                   ErrorCode.CreateNewUserAsyncException, ConstantData.Txn());
            }
        }
    }
}
