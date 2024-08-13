using service.Core.Entities.AccountManagement;

namespace service.Core.Interfaces.AccountManagement
{
    /// <summary>
    /// Defines the contract for a repository that manages user accounts.
    /// </summary>
    public interface IAccountRepository
    {
        /// <summary>
        /// Retrieves a list of all users asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of users.</returns>
        Task<List<User>> GetAllUsersAsync();
    }
}
