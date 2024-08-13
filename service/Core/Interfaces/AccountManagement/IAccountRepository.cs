using service.Core.Entities.AccountManagement;

namespace service.Core.Interfaces.AccountManagement
{
    /// <summary>
    /// Defines the contract for a repository that manages user accounts.
    /// </summary>
    public interface IAccountRepository
    {
        /// <summary>
        /// Asynchronously adds a new user to the system.
        /// </summary>
        /// <param name="user">The user object to be added.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the ID of the newly added user.</returns>
        Task<int> AddNewUserAsync(User user);

        /// <summary>
        /// Retrieves a list of all users asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of users.</returns>
        Task<List<User>> GetAllUsersAsync();
    }
}
