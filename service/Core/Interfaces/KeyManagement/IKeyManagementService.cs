using service.Core.Entities.KeyManagement;

namespace service.Core.Interfaces.KeyManagement
{
    /// <summary>
    /// Defines the interface for key management services.
    /// </summary>
    public interface IKeyManagementService
    {
        /// <summary>
        /// Gets a list of keys.
        /// </summary>
        /// <returns>A list of keys.</returns>
        Task<(int, List<Keys>)> GetKeysList();
    }
}
