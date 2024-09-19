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
        /// Creates a new user in the system.
        /// </summary>
        /// <param name="inCreateUser">An object containing the details of the new user.</param>
        /// <returns>A tuple containing the status code (1 for success, 0 for error, -1 for exception) and a message indicating the outcome.</returns>
        public async Task<(int, string)> CreateNewUser(InCreateUser inCreateUser)
        {
            try
            {
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
    }
}
