using service.Core.Entities.AccountManagement;

namespace service.Core.Interfaces.AccountManagement
{
    /// <summary>
    /// Defines the contract for a service that manages user accounts.
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// Retrieves a list of all users and the total count of users.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a tuple with the total count of users as the first element and a list of users as the second element.</returns>
        Task<(int, List<User>)> GetAllUsers();
    }
}
