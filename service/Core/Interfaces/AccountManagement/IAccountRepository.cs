using service.Core.Entities.AccountManagement;

namespace service.Core.Interfaces.AccountManagement
{
    /// <summary>
    /// Defines the contract for a repository that manages user accounts.
    /// </summary>
    public interface IAccountRepository
    {
        /// <summary>
        /// Gets a user's ID based on their username and email.
        /// </summary>
        /// <param name="userName">The username of the user.</param>
        /// <param name="email">The email of the user.</param>
        /// <returns>A Task that represents the asynchronous operation. The value of the Task is the user's ID, or 0 if no user is found.</returns>
        Task<int> GetUserUsernameEmailAsync(string userName, string email);

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

        /// <summary>
        /// Retrieve ID of email associated with email.
        /// </summary>
        /// <param name="email">Email associated with user.</param>
        /// <returns>An integer ID associated with email.</returns>
        Task<int> GetIdEmailAsync(string email);
    }
}
