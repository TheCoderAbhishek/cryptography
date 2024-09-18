using service.Core.Entities.AccountManagement;

namespace service.Core.Interfaces.UserManagement
{
    /// <summary>
    /// Defines the interface for the user management repository.
    /// </summary>
    public interface IUserManagementRepository
    {
        /// <summary>
        /// Gets a list of all users asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing a list of users.</returns>
        Task<List<User>> GetUsersAsync();

        /// <summary>
        /// Creates a new user asynchronously.
        /// </summary>
        /// <param name="user">The user object to create.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the ID of the newly created user.</returns>
        Task<int> CreateNewUserAsync(User user);
    }
}
