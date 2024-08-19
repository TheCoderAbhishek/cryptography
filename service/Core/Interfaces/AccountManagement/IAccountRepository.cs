using service.Core.Entities.AccountManagement;
using service.Core.Entities.Utility;

namespace service.Core.Interfaces.AccountManagement
{
    /// <summary>
    /// Defines the contract for a repository that manages user accounts.
    /// </summary>
    public interface IAccountRepository
    {
        /// <summary>
        /// Asynchronously retrieves a user from the database based on their email address.
        /// </summary>
        /// <param name="email">The email address of the user to retrieve.</param>
        /// <returns>A Task that represents the asynchronous operation. The task result contains the User object if found, otherwise null.</returns>
        Task<User?> GetUserEmailAsync(string email);

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

        /// <summary>
        /// Retrieves the ID and validity period of an OTP (One-Time Password) associated with a given email address.
        /// </summary>
        /// <param name="email">The email address for which to retrieve the OTP information.</param>
        /// <returns>A Task representing the asynchronous operation. The result of the Task is an OtpStorage object containing the ID and valid until timestamp of the OTP, or null if no OTP is found for the given email.</returns>
        Task<OtpStorage> GetIdValidUntilEmailAsync(string email);

        /// <summary>
        /// Updates the OTP details associated with the specified email address.
        /// </summary>
        /// <param name="otpStorage">The entity for which to update OTP details.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        Task<BaseResponse> UpdateOtpDetailsAsync(OtpStorage otpStorage);

        /// <summary>
        /// Retrieves OTP details associated with the specified email address.
        /// </summary>
        /// <param name="email">The email address for which to retrieve OTP details.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an OtpStorage object holding the retrieved OTP details, or null if no details were found.</returns>
        Task<OtpStorage> GetOtpDetailsEmailAsync(string email);

        /// <summary>
        /// Updates user details and unlocks the specified user in the system.
        /// </summary>
        /// <param name="user">The user object containing the updated details and unlock status.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows affected by the update operation.</returns>
        Task<int> UpdateUserDetailsUnlockUserAsync(User user);

        /// <summary>
        /// Updates the number of failed login attempts.
        /// </summary>
        /// <param name="user">The updated number of failed login attempts.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateFailedLoginAttemptsAsync(User user);

        /// <summary>
        /// Asynchronously updates the failed login attempts for a locked user and potentially unlocks them if applicable.
        /// </summary>
        /// <param name="user">The user object containing information about the locked user, including their login attempts.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateFailedLoginAttemptsLockedUserAsync(User user);
    }
}
