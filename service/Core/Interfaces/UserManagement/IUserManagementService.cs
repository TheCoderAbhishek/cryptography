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
        /// Creates a new user in the system.
        /// </summary>
        /// <param name="inCreateUser">An object containing the user's information for creation.</param>
        /// <returns>A tuple representing the operation's result. The first element is an integer indicating the status (1 for success, 0 for failure, -1 for exception). The second element is a string providing a detailed message about the outcome.</returns>
        Task<(int, string)> CreateNewUser(InCreateUser inCreateUser);
    }
}
