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
    }
}
