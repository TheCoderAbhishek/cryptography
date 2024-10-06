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
    }
}
