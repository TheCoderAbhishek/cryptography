using service.Core.Dto.UserManagement;
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

        /// <summary>
        /// Asynchronously updates user details based on the provided input.
        /// </summary>
        /// <param name="inUpdateUserDetails">The input object containing the updated user details.</param>
        /// <returns>A task representing the asynchronous operation. The task result indicates the number of rows affected by the update.</returns>
        Task<int> UpdateUserDetailsAsync(InUpdateUserDetails inUpdateUserDetails);

        /// <summary>
        /// Retrieves user details (excluding the current user's ID) based on the provided email and username.
        /// </summary>
        /// <param name="id">The user's id.</param>
        /// <param name="email">The user's email address.</param>
        /// <param name="username">The user's username.</param>
        /// <returns>A task representing the operation. The result is the user's ID if found, otherwise 0.</returns>
        Task<int> GetUserDetailsMailUsernameExceptCurrentIdAsync(int id, string email, string username);

        /// <summary>
        /// Restores a soft-deleted user with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the user to restore.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates the number of affected rows.</returns>
        Task<int> RestoreSoftDeletedUserAsync(int id);

        /// <summary>
        /// Asynchronously hard deletes a user with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the number of rows affected by the deletion.</returns>
        Task<int> HardDeleteUserAsync(int id);
    }
}
