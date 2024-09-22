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
        /// Gets a list of soft deleted users asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing a list of users.</returns>
        Task<List<User>> GetDeletedUsersAsync();

        /// <summary>
        /// Gets user details associated with specific ID.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>A task returns the User entity with user details.</returns>
        Task<User> GetUserDetailsByIdAsync(int id);

        /// <summary>
        /// Creates a new user asynchronously.
        /// </summary>
        /// <param name="user">The user object to create.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the ID of the newly created user.</returns>
        Task<int> CreateNewUserAsync(User user);

        /// <summary>
        /// Retrieves user details based on the provided email and username.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <param name="username">The username of the user.</param>
        /// <returns>A task that represents the asynchronous operation. The result of the task is an integer indicating the success or failure of the operation.</returns>
        Task<int> GetUserDetailsMailUsernameAsync(string email, string username);

        /// <summary>
        /// Locks or unlocks a user based on their ID.
        /// </summary>
        /// <param name="id">The ID of the user to lock or unlock.</param>
        /// <returns>
        /// The status code indicating the success or failure of the operation.
        /// 1: User successfully locked/unlocked.
        /// 0: User not found.
        /// -1: An unexpected error occurred.
        /// </returns>
        Task<int> LockUnlockUserAsync(int id);

        /// <summary>
        /// Soft deletes a user with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the user to soft delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<int> SoftDeleteUserAsync(int id);
    }
}
