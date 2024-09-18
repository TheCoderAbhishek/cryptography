using service.Core.Entities.AccountManagement;
using service.Core.Interfaces.UserManagement;

namespace service.Application.Repository.UserManagement
{
    /// <summary>
    /// Implements the <see cref="IUserManagementService"/> interface for user management operations.
    /// </summary>
    public class UserManagementService(ILogger<UserManagementService> logger, IUserManagementRepository userManagementRepository) : IUserManagementService
    {
        private readonly ILogger<UserManagementService> _logger = logger;
        private readonly IUserManagementRepository _userManagementRepository = userManagementRepository;

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
    }
}
