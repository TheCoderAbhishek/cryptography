using Serilog;
using service.Controllers;
using service.Core.Entities.AccountManagement;
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
    public class AccountRepository(ILogger<AccountController> logger, ICommonDbHander commonDbHander) : IAccountRepository
    {
        private readonly ILogger<AccountController> _logger = logger;
        private readonly ICommonDbHander _commonDbHander = commonDbHander;

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
    }
}
