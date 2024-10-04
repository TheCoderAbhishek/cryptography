using service.Core.Dto.UserManagement;
using service.Core.Entities.AccountManagement;

namespace service.Core.Interfaces.UserManagement
{
    /// <summary>
    /// Defines the interface for user management services.
    /// </summary>
    public interface IUserManagementService
    {
        /// <summary>
        /// Retrieves a list of all users and the total count of users.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a tuple with the total count of users as the first element and a list of users as the second element.</returns>
        Task<(int, List<User>)> GetAllUsers();

        /// <summary>
        /// Retrieves a list of soft deleted users and the total count of users.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a tuple with the total count of soft deleted users as the first element and a list of soft deleted users as the second element.</returns>
        Task<(int, List<User>)> GetSoftDeletedUsers();

        /// <summary>
        /// Creates a new user in the system.
        /// </summary>
        /// <param name="inCreateUser">An object containing the user's information for creation.</param>
        /// <returns>A tuple representing the operation's result. The first element is an integer indicating the status (1 for success, 0 for failure, -1 for exception). The second element is a string providing a detailed message about the outcome.</returns>
        Task<(int, string)> CreateNewUser(InCreateUser inCreateUser);

        /// <summary>
        /// Locks or unlocks a user based on their ID.
        /// </summary>
        /// <param name="id">The ID of the user to lock or unlock.</param>
        /// <returns>
        /// A tuple containing the status code and a message indicating the success or failure of the operation.
        /// The status code is 1 if the user was successfully locked/unlocked, 0 if the user was not found, or -1 if an unexpected error occurred.
        /// </returns>
        Task<(int, string)> LockUnlockUser(int id);

        /// <summary>
        /// Soft delete an user based on their ID.
        /// </summary>
        /// <param name="id">The ID of the user to soft delete.</param>
        /// <returns>A tuple to return status and message.</returns>
        Task<(int, string)> SoftDeleteUser(int id);

        /// <summary>
        /// Updates the user details based on the provided input.
        /// </summary>
        /// <param name="inUpdateUserDetails">The input object containing the updated user details.</param>
        /// <returns>A tuple containing the updated user ID and a success message.</returns>
        Task<(int, string)> UpdateUserDetails(InUpdateUserDetails inUpdateUserDetails);
    }
}
