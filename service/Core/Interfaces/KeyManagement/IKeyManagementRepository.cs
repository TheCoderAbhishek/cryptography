using service.Core.Entities.KeyManagement;

namespace service.Core.Interfaces.KeyManagement
{
    /// <summary>
    /// Defines the interface for interacting with key management data.
    /// </summary>
    public interface IKeyManagementRepository
    {
        /// <summary>
        /// Asynchronously retrieves a list of keys.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, returning a list of keys.</returns>
        Task<List<Keys>> GetKeysListAsync();

        /// <summary>
        /// Creates a new key asynchronously.
        /// </summary>
        /// <param name="key">The key to create.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the newly created key ID.</returns>
        Task<int> CreateKeyAsync(Keys key);

        /// <summary>
        /// Checks if a given key name is unique within the system.
        /// </summary>
        /// <param name="keyName">The key name to be checked.</param>
        /// <returns>A task that returns 1 if the key name is unique, 0 if it is not unique, and throws an exception if there is an error.</returns>
        Task<int> CheckUniqueKeyName(string keyName);
    }
}
