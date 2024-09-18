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
    }
}
