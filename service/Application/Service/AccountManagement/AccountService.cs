using service.Core.Entities.AccountManagement;
using service.Core.Interfaces.AccountManagement;

namespace service.Application.Service.AccountManagement
{
    /// <summary>
    /// Represents the account service for managing user accounts.
    /// </summary>
    public class AccountService(ILogger<AccountService> logger, IAccountRepository accountRepository) : IAccountService
    {
        private readonly ILogger<AccountService> _logger = logger;
        private readonly IAccountRepository _accountRepository = accountRepository;

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns>A tuple containing the status code and a list of users.</returns>
        public async Task<(int, List<User>)> GetAllUsers()
        {
            try
            {
                List<User> user = await _accountRepository.GetAllUsersAsync();
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
