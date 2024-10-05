using Konscious.Security.Cryptography;
using service.Core.Dto.UserManagement;
using service.Core.Entities.AccountManagement;
using service.Core.Interfaces.UserManagement;
using System.Security.Cryptography;
using System.Text;

namespace service.Application.Repository.UserManagement
{
    /// <summary>
    /// Implements the <see cref="IUserManagementService"/> interface for user management operations.
    /// </summary>
    public class UserManagementService(ILogger<UserManagementService> logger, IUserManagementRepository userManagementRepository) : IUserManagementService
    {
        private readonly ILogger<UserManagementService> _logger = logger;
        private readonly IUserManagementRepository _userManagementRepository = userManagementRepository;

        #region Private Methods for Support
        /// <summary>
        /// Generates a random base64 encoded salt string of length 16 bytes.
        /// </summary>
        /// <returns>A base64 encoded string representing the random salt.</returns>
        private static string GetGenerateSalt()
        {
            var rng = RandomNumberGenerator.Create();
            var buffer = new byte[16];
            rng.GetBytes(buffer);
            return Convert.ToBase64String(buffer);
        }

        /// <summary>
        /// Hashes a password using SHA256 with a provided salt.
        /// </summary>
        /// <param name="password">The password to be hashed.</param>
        /// <param name="salt">The salt to be used for hashing.</param>
        /// <returns>A base64 encoded string representing the hashed password.</returns>
        private static string HashPassword(string password, string salt)
        {
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = Convert.FromBase64String(salt),
                DegreeOfParallelism = 8,
                MemorySize = 65536,
                Iterations = 4
            };

            var hash = argon2.GetBytes(16);
            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Generates a new unique client identifier using a Guid.
        /// </summary>
        /// <returns>A string representing the newly generated client ID.</returns>
        private static string GenerateClientId()
        {
            return Guid.NewGuid().ToString();
        }
        #endregion

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns>A tuple containing the status code and a list of users.</returns>
        public async Task<(int, List<User>)> GetAllUsers()
        {
            try
            {
                List<User> user = await _userManagementRepository.GetUsersAsync();
                if (user == null || user.Count == 0)
                {
                    _logger.LogError("No users found.");
                    return (0, user)!;
                }
                else
                {
                    _logger.LogInformation("{User} users retrieved successfully.", user);
                    return (1, user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users: {Message}", ex.Message);
                return (-1, null)!;
            }
        }

        /// <summary>
        /// Retrieves a list of soft-deleted users from the database.
        /// </summary>
        /// <returns>A list of soft-deleted User objects, or null if an error occurred.</returns>
        public async Task<(int, List<User>)> GetSoftDeletedUsers()
        {
            try
            {
                List<User> user = await _userManagementRepository.GetDeletedUsersAsync();
                if (user == null || user.Count == 0)
                {
                    _logger.LogError("No soft deleted users found.");
                    return (0, user)!;
                }
                else
                {
                    _logger.LogInformation("List of soft deleted users retrieved successfully.");
                    return (1, user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving soft deleted users: {Message}", ex.Message);
                return (-1, null)!;
            }
        }

        /// <summary>
        /// Creates a new user in the system.
        /// </summary>
        /// <param name="inCreateUser">An object containing the details of the new user.</param>
        /// <returns>A tuple containing the status code (1 for success, 0 for error, -1 for exception) and a message indicating the outcome.</returns>
        public async Task<(int, string)> CreateNewUser(InCreateUser inCreateUser)
        {
            try
            {
                int duplicateRecord = await _userManagementRepository.GetUserDetailsMailUsernameAsync(inCreateUser.Email!, inCreateUser.UserName!);

                if (duplicateRecord == 1)
                {
                    _logger.LogWarning("Duplicate email found: {Email}", inCreateUser.Email);
                    return (2, "Email address already exists.");
                }

                if (duplicateRecord == 2)
                {
                    _logger.LogWarning("Duplicate username found: {UserName}", inCreateUser.UserName);
                    return (3, "Username already exists.");
                }

                var salt = GetGenerateSalt();
                var hashedPassword = HashPassword(inCreateUser.Password!, salt);

                var newUser = new User
                {
                    UserId = GenerateClientId(),
                    Name = inCreateUser.Name,
                    UserName = inCreateUser.UserName,
                    Email = inCreateUser.Email,
                    Password = hashedPassword,
                    CreatedOn = DateTime.Now,
                    IsAdmin = true,
                    IsActive = true,
                    IsLocked = true,
                    IsDeleted = false,
                    DeletedStatus = DeletedState.NotDeleted,
                    LoginAttempts = 0,
                    RoleId = (RoleId?)inCreateUser.RoleId,
                    Salt = salt
                };

                int userId = await _userManagementRepository.CreateNewUserAsync(newUser);

                if (userId > 0)
                {
                    _logger.LogInformation("New user created successfully.");
                    return (1, "New user created successfully.");
                }
                else
                {
                    _logger.LogError("An error while creating new user.");
                    return (0, "An error while creating new user.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users: {Message}", ex.Message);
                return (-1, $"{ex.Message}");
            }
        }

        /// <summary>
        /// Locks or unlocks a user based on their ID.
        /// </summary>
        /// <param name="id">The ID of the user to lock or unlock.</param>
        /// <returns>
        /// A tuple containing the status code and a message indicating the success or failure of the operation.
        /// The status code is 1 if the user was successfully locked/unlocked, 0 if the operation failed, or -1 if an unexpected error occurred.
        /// </returns>
        public async Task<(int, string)> LockUnlockUser(int id)
        {
            try
            {
                int result = await _userManagementRepository.LockUnlockUserAsync(id);

                if (result > 0)
                {
                    _logger.LogInformation("User with ID {Id} was successfully locked/unlocked.", id);
                    return (result, "User successfully locked/unlocked.");
                }
                else
                {
                    _logger.LogWarning("Failed to lock/unlock user with ID {Id}.", id);
                    return (0, $"Failed to lock/unlock user.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users: {Message}", ex.Message);
                return (-1, $"{ex.Message}");
            }
        }

        /// <summary>
        /// Soft deletes a user with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the user to soft delete.</param>
        /// <returns>A tuple containing the result of the operation and a message indicating success or failure.
        /// The first element of the tuple is an integer representing the number of affected rows.
        /// The second element is a string containing the operation result message.
        /// </returns>
        public async Task<(int, string)> SoftDeleteUser(int id)
        {
            try
            {
                User user = await _userManagementRepository.GetUserDetailsByIdAsync(id);

                if (user != null)
                {
                    if (user.IsDeleted == false)
                    {
                        int result = await _userManagementRepository.SoftDeleteUserAsync(id);

                        if (result > 0)
                        {
                            _logger.LogInformation("User with ID {Id} was successfully soft deleted.", id);
                            return (result, "User successfully soft deleted.");
                        }
                        else
                        {
                            _logger.LogWarning("Failed to soft delete user with ID {Id}.", id);
                            return (0, $"Failed to soft delete user.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Failed to soft delete user with ID {Id} because user already in soft deleted state.", id);
                        return (2, $"Failed to soft delete user because user already in soft deleted state.");
                    }
                }
                else
                {
                    _logger.LogError("Invalid {Id} id provided for user.", id);
                    return (3, "Failed to retrieve user details from table.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while soft deleting user: {Message}", ex.Message);
                return (-1, $"{ex.Message}");
            }
        }

        /// <summary>
        /// Updates user details in the database asynchronously.
        /// </summary>
        /// <param name="inUpdateUserDetails">The input object containing the updated user details.</param>
        /// <returns>A tuple containing the following information:
        ///     - `int`: The number of rows affected by the update (0 if no update happened).
        ///     - `string`: A message indicating the outcome of the update operation.
        /// </returns>
        /// <exception cref="Exception">An exception may be thrown if an error occurs during the update process.</exception>
        public async Task<(int, string)> UpdateUserDetails(InUpdateUserDetails inUpdateUserDetails)
        {
            try
            {
                User user = await _userManagementRepository.GetUserDetailsByIdAsync(inUpdateUserDetails.Id);

                if (user != null)
                {
                    int duplicateRecord = await _userManagementRepository.GetUserDetailsMailUsernameExceptCurrentIdAsync(inUpdateUserDetails.Id, inUpdateUserDetails.Email!, inUpdateUserDetails.UserName!);

                    if (duplicateRecord == 1)
                    {
                        _logger.LogWarning("Duplicate email found: {Email}", inUpdateUserDetails.Email);
                        return (2, "Email address already exists.");
                    }

                    if (duplicateRecord == 2)
                    {
                        _logger.LogWarning("Duplicate username found: {UserName}", inUpdateUserDetails.UserName);
                        return (3, "Username already exists.");
                    }

                    if (duplicateRecord == 3)
                    {
                        _logger.LogWarning("Duplicate username and email found: {UserName}", inUpdateUserDetails.UserName);
                        return (4, "Username and email already exists.");
                    }

                    int status = await _userManagementRepository.UpdateUserDetailsAsync(inUpdateUserDetails);

                    if (status > 0)
                    {
                        _logger.LogInformation("User details updated successfully.");
                        return (1, "User details updated successfully.");
                    }
                    else
                    {
                        _logger.LogError("An error occurred updating user details.");
                        return (0, "Error occurred while updating user details.");
                    }
                }
                else
                {
                    _logger.LogError("An error occurred getting user details.");
                    return (4, "User not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user details: {Message}", ex.Message);
                return (-1, $"{ex.Message}");
            }
        }

        /// <summary>
        /// Asynchronously hard deletes a user with the specified ID and returns a tuple containing the number of rows affected and an error message if any occurred.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is a tuple containing the number of rows affected by the deletion and an error message if any occurred.</returns>
        public async Task<(int, string)> HardDeleteUser(int id)
        {
            try
            {
                User user = await _userManagementRepository.GetUserDetailsByIdAsync(id);

                if (user != null)
                {
                    int status = await _userManagementRepository.HardDeleteUserAsync(id);

                    if (status > 0)
                    {
                        _logger.LogInformation("User deleted successfully.");
                        return (1, "User deleted successfully.");
                    }
                    else
                    {
                        _logger.LogError("An error occurred deleting user.");
                        return (0, "Error occurred while deleting user.");
                    }
                }
                else
                {
                    _logger.LogError("An error occurred getting user details.");
                    return (2, "User not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while hard delete user: {Message}", ex.Message);
                return (-1, $"{ex.Message}");
            }
        }
    }
}
