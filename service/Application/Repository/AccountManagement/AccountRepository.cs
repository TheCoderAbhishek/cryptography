using service.Controllers;
using service.Core.Entities.AccountManagement;
using service.Core.Entities.Utility;
using service.Core.Enums;
using service.Core.Interfaces.AccountManagement;
using service.Core.Interfaces.Utility;
using service.Infrastructure.Dependency;
using service.Infrastructure.Queries.Account;

namespace service.Application.Repository.AccountManagement
{
    /// <summary>
    /// Represents a repository for managing user accounts.
    /// </summary>
    public class AccountRepository(ILogger<AccountRepository> logger, ICommonDbHander commonDbHander) : IAccountRepository
    {
        private readonly ILogger<AccountRepository> _logger = logger;
        private readonly ICommonDbHander _commonDbHander = commonDbHander;

        /// <summary>
        /// This method asynchronously adds a new user to the system.
        /// </summary>
        /// <param name="user">The User object containing information for the new user.</param>
        /// <returns>An integer representing the status of the operation. 
        ///         Typically, a successful operation returns 1, while failures might return different values 
        ///         depending on the specific implementation.</returns>
        /// <exception cref="CustomException">Thrown if an error occurs while adding the user. 
        ///         The exception includes details about the error and relevant error codes.</exception>
        public async Task<int> AddNewUserAsync(User user)
        {
            string query = AccountQueries.AddNewUser;

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

                BaseResponse baseResponse = await _commonDbHander.AddDataReturnLatestId(query, "New user added successfully.", "An error occurred while adding the new user. Please try again.", "A user with the same username or email already exists.", ErrorCode.AddNewUserAsyncError, ConstantData.Txn(), parameters);

                return baseResponse.Status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding new user. {Message}", ex.Message);
                throw new CustomException("Error adding new user from the account repository.", ex,
                                   ErrorCode.AddNewUserAsyncError, ConstantData.Txn());
            }
        }

        /// <summary>
        /// Retrieves a list of all users asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of users.</returns>
        public async Task<List<User>> GetAllUsersAsync()
        {
            string query = AccountQueries.GetAllUsers;

            try
            {
                List<User> users = await _commonDbHander.GetData<User>(query, "Fetched all users successfully",
                                                            "Error fetching users", ErrorCode.GetAllUsersAsyncError, ConstantData.Txn());

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
                                   ErrorCode.GetAllUsersAsyncError, ConstantData.Txn());
            }
        }

        /// <summary>
        /// Retrieve ID of email associated with email.
        /// </summary>
        /// <param name="email">Email associated with user.</param>
        /// <returns>An integer ID associated with email.</returns>
        public async Task<int> GetIdEmailAsync(string email)
        {
            try
            {
                string query = AccountQueries.GetIdEmail;

                var parameter = new
                {
                    Email = email,
                };

                int id = await _commonDbHander.GetSingleData<int>(query, $"Getting ID associated with email {email}", $"An error occurred while getting ID associated with email {email}", ErrorCode.GetIdEmailAsyncError, ConstantData.Txn(), parameter);
                return id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching ID associated with email. {Message}", ex.Message);
                throw new CustomException("Error fetching ID associated with email from the account repository.", ex,
                                   ErrorCode.GetIdEmailAsyncError, ConstantData.Txn());
            }
        }
    }
}
