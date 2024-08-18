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
        /// Asynchronously retrieves a user from the database based on their email address.
        /// </summary>
        /// <param name="email">The email address of the user to retrieve.</param>
        /// <returns>A Task that represents the asynchronous operation. The task result contains the User object if found, otherwise null.</returns>
        /// <exception cref="CustomException">Thrown if an error occurs while retrieving the user details.</exception>
        public async Task<User?> GetUserEmailAsync(string email)
        {
            string query = AccountQueries.GetUserEmail;

            try
            {
                var parameter = new
                {
                    Email = email
                };

                User user = await _commonDbHander.GetSingleData<User>(query,
                    $"Getting user details associated with email {email}",
                    $"An error occurred while getting user details associated with email {email}",
                    ErrorCode.GetUserEmailAsyncError, ConstantData.Txn(), parameter);

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while getting user details based upon email. {Message}", ex.Message);
                throw new CustomException("Error getting user details based upon email from the account repository.", ex,
                                   ErrorCode.GetUserEmailAsyncError, ConstantData.Txn());
            }
        }

        /// <summary>
        /// Retrieves the ID of a user based on their username and email address.
        /// </summary>
        /// <param name="userName">The username of the user.</param>
        /// <param name="email">The email address of the user.</param>
        /// <returns>A Task representing the asynchronous operation. The result of the Task is the ID of the user, or 0 if no user is found.</returns>
        /// <exception cref="CustomException">Thrown when an error occurs during the retrieval process.</exception>
        public async Task<int> GetUserUsernameEmailAsync(string userName, string email)
        {
            string query = AccountQueries.GetUserUsernameEmail;

            try
            {
                var parameter = new
                {
                    UserName = userName,
                    Email = email
                };

                int id = await _commonDbHander.GetSingleData<int>(query,
                    $"Getting ID associated with username or email {email}",
                    $"An error occurred while getting ID associated with username or email {email}",
                    ErrorCode.GetUserUsernameEmailAsyncError, ConstantData.Txn(), parameter);

                return id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while getting Id based upon username or email. {Message}", ex.Message);
                throw new CustomException("Error getting Id based upon username or email from the account repository.", ex,
                                   ErrorCode.GetUserUsernameEmailAsyncError, ConstantData.Txn());
            }
        }

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

                int id = await _commonDbHander.GetSingleData<int>(query, $"Getting ID associated with email {email}",
                    $"An error occurred while getting ID associated with email {email}",
                    ErrorCode.GetIdEmailAsyncError,
                    ConstantData.Txn(), parameter);

                return id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching ID associated with email. {Message}", ex.Message);
                throw new CustomException("Error fetching ID associated with email from the account repository.", ex,
                                   ErrorCode.GetIdEmailAsyncError, ConstantData.Txn());
            }
        }

        /// <summary>
        /// Retrieves the ID and validity period of an OTP (One-Time Password) associated with the provided email address from the database.
        /// </summary>
        /// <param name="email">The email address for which to retrieve the OTP information.</param>
        /// <returns>A Task representing the asynchronous operation. The result of the Task is an OtpStorage object containing the ID and valid until timestamp of the OTP, or null if no OTP is found for the given email.</returns>
        /// <exception cref="CustomException">Thrown if an error occurs during the retrieval process from the database.</exception>
        public async Task<OtpStorage> GetIdValidUntilEmailAsync(string email)
        {
            try
            {
                string query = AccountQueries.GetIdValidUntilEmail;

                var parameter = new
                {
                    Email = email
                };

                OtpStorage otpDetails = await _commonDbHander.GetSingleData<OtpStorage>(query,
                    $"Getting OTP details associated with email {email}",
                    $"An error occurred while getting OTP details associated with email {email}",
                    ErrorCode.GetIdValidUntilEmailAsyncError, ConstantData.Txn(), parameter);

                return otpDetails;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching OTP details associated with email. {Message}", ex.Message);
                throw new CustomException("Error fetching OTP details associated with email from the account repository.", ex,
                                   ErrorCode.GetIdValidUntilEmailAsyncError, ConstantData.Txn());
            }
        }

        /// <summary>
        /// Asynchronously updates the OTP details in the storage.
        /// </summary>
        /// <param name="otpStorage">The OTP storage object containing the updated details.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <exception cref="CustomException">Thrown when an error occurs during the update process.</exception>
        public async Task<BaseResponse> UpdateOtpDetailsAsync(OtpStorage otpStorage)
        {
            try
            {
                string query = AccountQueries.UpdateOtpInValid;

                var parameter = new
                {
                    otpStorage.Email,
                    otpStorage.GeneratedOn,
                    otpStorage.ValidUntil,
                    otpStorage.Otp
                };

                BaseResponse baseResponse = await _commonDbHander.AddUpdateDeleteData(query, "OTP details updated successfully.",
                    "Error updating OTP details associated with email from the account repository.",
                    "Not applicable in this context.",
                    ErrorCode.UpdateOtpDetailsAsyncError,
                    ConstantData.Txn(), parameter);

                return baseResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating OTP details associated with email. {Message}", ex.Message);
                throw new CustomException("Error updating OTP details associated with email from the account repository.", ex,
                                   ErrorCode.UpdateOtpDetailsAsyncError, ConstantData.Txn());
            }
        }

        /// <summary>
        /// Retrieves OTP details associated with the specified email address from the database.
        /// </summary>
        /// <param name="email">The email address for which to retrieve OTP details.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an OtpStorage object holding the retrieved OTP details.</returns>
        /// <exception cref="CustomException">Thrown when an error occurs while fetching OTP details from the database.</exception>
        public async Task<OtpStorage> GetOtpDetailsEmailAsync(string email)
        {
            try
            {
                string query = AccountQueries.GetOtpDetails;

                var parameters = new
                {
                    @Email = email
                };

                OtpStorage otpStorage = await _commonDbHander.GetSingleData<OtpStorage>(query, "OTP details fetched successfully.",
                    "No OTP details found for the provided email.",
                    ErrorCode.GetOtpDetailsEmailAsyncError, ConstantData.Txn(),
                    parameters);

                return otpStorage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching OTP details associated with email. {Message}", ex.Message);
                throw new CustomException("Error occurred while fetching OTP details associated with email from the account repository.", ex,
                                   ErrorCode.GetOtpDetailsEmailAsyncError, ConstantData.Txn());
            }
        }

        /// <summary>
        /// Unlocks a user account in the system by updating their 'IsLocked' status.
        /// </summary>
        /// <param name="user">The user object containing the email and the updated 'IsLocked' status (expected to be false to unlock).</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows affected (1 if successful).</returns>
        /// <exception cref="CustomException">Thrown when an error occurs during the unlock operation.</exception>
        public async Task<int> UpdateUserDetailsUnlockUserAsync(User user)
        {
            try
            {
                string query = AccountQueries.UnlockUser;

                var parameters = new
                {
                    user.IsLocked,
                    user.Email,
                };

                await _commonDbHander.AddUpdateDeleteData(query, "User unlocked successfully.",
                    "Failed to unlock user. User not found or already unlocked.",
                    "",
                    ErrorCode.UpdateUserDetailsUnlockUserAsyncError, ConstantData.Txn(), parameters);

                return 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while unlocking user with email. {Message}", ex.Message);
                throw new CustomException("Error occurred while unlocking user with email from the account repository.", ex,
                                   ErrorCode.UpdateUserDetailsUnlockUserAsyncError, ConstantData.Txn());
            }
        }
    }
}
