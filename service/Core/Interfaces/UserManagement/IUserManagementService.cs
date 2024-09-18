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
    }
}
