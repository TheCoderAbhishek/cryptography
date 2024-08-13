using service.Core.Dto.AccountManagement;
using service.Core.Entities.AccountManagement;

namespace service.Core.Interfaces.AccountManagement
{
    /// <summary>
    /// Defines the contract for a service that manages user accounts.
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// Asynchronously adds a new user to the system and returns the user ID and a status code.
        /// </summary>
        /// <param name="inAddUserDto">The user data object containing information for the new user.</param>
        /// <returns>A task representing the asynchronous operation. The task result is a tuple containing the ID of the newly added user and a status code.</returns>
        Task<(int, int)> AddNewUser(InAddUserDto inAddUserDto);

        /// <summary>
        /// Retrieves a list of all users and the total count of users.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a tuple with the total count of users as the first element and a list of users as the second element.</returns>
        Task<(int, List<User>)> GetAllUsers();
    }
}
