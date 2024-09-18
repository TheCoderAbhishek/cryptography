using service.Core.Entities.AccountManagement;
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
    }
}
